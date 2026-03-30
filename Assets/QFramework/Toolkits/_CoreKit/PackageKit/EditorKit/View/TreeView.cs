/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class AssetTree
    {
        public AssetTree()
        {
            Root = new TreeNode<TreeAssetData>(null);
        }

        public TreeNode<TreeAssetData> Root { get; }

        public void Clear()
        {
            Root.Clear();
        }

        public void AddAsset(string guid, HashSet<string> incluedPathes)
        {
            if (string.IsNullOrEmpty(guid)) return;

            var node = Root;

            var assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if (assetPath.StartsWith("Packages")) return;

            int startIndex = 0, length = assetPath.Length;

            var isSelected = incluedPathes.Contains(assetPath);

            var isExpanded = incluedPathes.Any(path => path.Contains(assetPath));

            while (startIndex < length)
            {
                var endIndex = assetPath.IndexOf('/', startIndex);
                var subLength = endIndex == -1 ? length - startIndex : endIndex - startIndex;
                var directory = assetPath.Substring(startIndex, subLength);

                var pathNode = new TreeAssetData(endIndex == -1 ? guid : null, directory,
                    assetPath.Substring(0, endIndex == -1 ? length : endIndex), node.Level == 0 || isExpanded,
                    isSelected);

                var child = node.FindInChildren(pathNode);

                if (child == null) child = node.AddChild(pathNode);

                node = child;
                startIndex += subLength + 1;
            }
        }
    }

    public class TreeAssetData : ITreeIMGUIData
    {
        public readonly string fullPath;
        public readonly string guid;
        public readonly string path;

        public TreeAssetData(string guid, string path, string fullPath, bool isExpanded, bool isSelected)
        {
            this.guid = guid;
            this.path = path;
            this.fullPath = fullPath;
            this.isExpanded = isExpanded;
            this.isSelected = isSelected;
        }

        public bool isSelected { get; set; }
        public bool isExpanded { get; set; }

        public override string ToString()
        {
            return path;
        }

        public override int GetHashCode()
        {
            return path.GetHashCode() + 10;
        }

        public override bool Equals(object obj)
        {
            var node = obj as TreeAssetData;
            return node != null && node.path == path;
        }

        public bool Equals(TreeAssetData node)
        {
            return node.path == path;
        }
    }

    public class AssetTreeIMGUI : TreeIMGUI<TreeAssetData>
    {
        public AssetTreeIMGUI(TreeNode<TreeAssetData> root) : base(root)
        {
        }

        protected override void OnDrawTreeNode(Rect rect, TreeNode<TreeAssetData> node, bool selected, bool focus)
        {
            var labelContent = new GUIContent(node.Data.path, AssetDatabase.GetCachedIcon(node.Data.fullPath));

            if (!node.IsLeaf)
                node.Data.isExpanded = EditorGUI.Foldout(new Rect(rect.x - 12, rect.y, 12, rect.height),
                    node.Data.isExpanded, GUIContent.none);

            EditorGUI.BeginChangeCheck();
            node.Data.isSelected = EditorGUI.ToggleLeft(rect, labelContent, node.Data.isSelected);
        }
    }

    public class TreeIMGUI<T> where T : ITreeIMGUIData
    {
        private readonly TreeNode<T> _root;
        private int _controlID;

        private Rect _controlRect;
        private float _drawY;
        private float _height;
        private TreeNode<T> _selected;

        public TreeIMGUI(TreeNode<T> root)
        {
            _root = root;
        }

        public void DrawTreeLayout()
        {
            _height = 0;
            _drawY = 0;
            _root.Traverse(OnGetLayoutHeight);

            _controlRect = EditorGUILayout.GetControlRect(false, _height);
            _controlID = GUIUtility.GetControlID(FocusType.Passive, _controlRect);
            _root.Traverse(OnDrawRow);
        }

        protected virtual float GetRowHeight(TreeNode<T> node)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected virtual bool OnGetLayoutHeight(TreeNode<T> node)
        {
            if (node.Data == null) return true;

            _height += GetRowHeight(node);
            return node.Data.isExpanded;
        }

        protected virtual bool OnDrawRow(TreeNode<T> node)
        {
            if (node.Data == null) return true;

            float rowIndent = 14 * node.Level;
            var rowHeight = GetRowHeight(node);

            var rowRect = new Rect(0, _controlRect.y + _drawY, _controlRect.width, rowHeight);
            var indentRect = new Rect(rowIndent, _controlRect.y + _drawY, _controlRect.width - rowIndent, rowHeight);

            // render
            if (_selected == node) EditorGUI.DrawRect(rowRect, Color.gray);

            OnDrawTreeNode(indentRect, node, _selected == node, false);

            // test for events
            var eventType = Event.current.GetTypeForControl(_controlID);
            if (eventType == EventType.MouseUp && rowRect.Contains(Event.current.mousePosition))
            {
                _selected = node;

                GUI.changed = true;
                Event.current.Use();
            }

            _drawY += rowHeight;

            return node.Data.isExpanded;
        }

        protected virtual void OnDrawTreeNode(Rect rect, TreeNode<T> node, bool selected, bool focus)
        {
            var labelContent = new GUIContent(node.Data.ToString());

            if (!node.IsLeaf)
                node.Data.isExpanded = EditorGUI.Foldout(new Rect(rect.x - 12, rect.y, 12, rect.height),
                    node.Data.isExpanded, GUIContent.none);

            EditorGUI.LabelField(rect, labelContent, selected ? EditorStyles.whiteLabel : EditorStyles.label);
        }
    }

    public interface ITreeIMGUIData
    {
        bool isExpanded { get; set; }
    }

    public class TreeNode<T>
    {
        public delegate bool TraversalDataDelegate(T data);

        public delegate bool TraversalNodeDelegate(TreeNode<T> node);

        private readonly List<TreeNode<T>> _children;

        public TreeNode(T data)
        {
            Data = data;
            _children = new List<TreeNode<T>>();
            Level = 0;
        }

        public TreeNode(T data, TreeNode<T> parent) : this(data)
        {
            Parent = parent;
            Level = Parent != null ? Parent.Level + 1 : 0;
        }

        public int Level { get; }

        public int Count => _children.Count;

        public bool IsRoot => Parent == null;

        public bool IsLeaf => _children.Count == 0;

        public T Data { get; }

        public TreeNode<T> Parent { get; }

        public TreeNode<T> this[int key] => _children[key];

        public void Clear()
        {
            _children.Clear();
        }

        public TreeNode<T> AddChild(T value)
        {
            var node = new TreeNode<T>(value, this);
            _children.Add(node);

            return node;
        }

        public bool HasChild(T data)
        {
            return FindInChildren(data) != null;
        }

        public TreeNode<T> FindInChildren(T data)
        {
            int i = 0, l = Count;
            for (; i < l; ++i)
            {
                var child = _children[i];
                if (child.Data.Equals(data)) return child;
            }

            return null;
        }

        public bool RemoveChild(TreeNode<T> node)
        {
            return _children.Remove(node);
        }

        public void Traverse(TraversalDataDelegate handler)
        {
            if (handler(Data))
            {
                int i = 0, l = Count;
                for (; i < l; ++i) _children[i].Traverse(handler);
            }
        }

        public void Traverse(TraversalNodeDelegate handler)
        {
            if (handler(this))
            {
                int i = 0, l = Count;
                for (; i < l; ++i) _children[i].Traverse(handler);
            }
        }
    }
}
#endif