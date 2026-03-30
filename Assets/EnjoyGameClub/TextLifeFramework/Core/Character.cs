/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System;
using TMPro;
using UnityEngine;

namespace EnjoyGameClub.TextLifeFramework.Core
{
    /// <summary>
    ///     描述字符的（如文本、UI元素）位置、旋转、缩放等信息的结构体。
    /// </summary>
    /// <remarks>
    ///     A structure describing the position, rotation, scaling, and other transform data of 2D objects such as text or UI
    ///     elements.
    /// </remarks>
    public struct Transform
    {
        // Transform Base Data
        public Vector3[] OriginalVertices;
        public Vector3[] Vertices;
        public TMP_Text TMPComponent;

        // Position
        private Vector3 _runtimeLocalPosition;
        private Vector3 _localPosition;

        public Vector3 LocalPosition
        {
            get => _localPosition;
            set => Move(value);
        }

        public Vector3 ScreenPosition
        {
            get => GetScreenPosition();
            set => SetScreenPosition(value);
        }

        // Rotation
        private Quaternion _runtimeRotation;
        private Quaternion _rotation;

        public Quaternion Rotation
        {
            get => _rotation;
            set => Rotate(value);
        }

        // EulerAnlge
        private Vector3 _runtimeAngle;
        private Vector3 _eulerAngle;

        public Vector3 EulerAngle
        {
            get => _eulerAngle;
            set => Rotate(value);
        }

        // Scale
        private Vector2 _runtimeScale;
        private Vector2 _localScale;

        public Vector2 LocalScale
        {
            get => _localScale;
            set => Scale(value);
        }

        public float Width => Vector3.Distance(Vertices[3], Vertices[0]);
        public float Height => Vector3.Distance(Vertices[1], Vertices[0]);

        /// <summary>
        ///     移动顶点位置。
        /// </summary>
        /// <remarks>
        ///     Move vertices position.
        /// </remarks>
        /// <param name="offset">位移量。</param>
        public void Move(Vector3 offset)
        {
            for (var i = 0; i < Vertices.Length; i++) Vertices[i] += offset;

            _runtimeLocalPosition += offset;
        }

        /// <summary>
        ///     缩放网格。
        /// </summary>
        /// <remarks>
        ///     Scale mesh.
        /// </remarks>
        /// <param name="scale">缩放比例。</param>
        /// <param name="pivotPercentage">可选的缩放枢轴点（0到1的比例）。</param>
        public void Scale(Vector2 scale, Vector2? pivotPercentage = null)
        {
            var pivot = pivotPercentage.HasValue
                ? new Vector3(
                    Mathf.Lerp(Vertices[0].x, Vertices[2].x, pivotPercentage.Value.x),
                    Mathf.Lerp(Vertices[0].y, Vertices[2].y, pivotPercentage.Value.y),
                    0
                )
                : GetComponentPosition();
            for (var i = 0; i < Vertices.Length; i++)
                // 将顶点相对 Pivot 进行缩放
                Vertices[i] = pivot + Vector3.Scale(Vertices[i] - pivot, new Vector3(scale.x, scale.y, 1));

            _runtimeScale *= scale;
        }

        /// <summary>
        ///     旋转网格
        /// </summary>
        /// <remarks>
        ///     Rotate mesh.
        /// </remarks>
        /// <param name="angle">旋转角度。</param>
        /// <param name="pivotPercentage">可选的旋转枢轴点（0到1的比例）。</param>
        [Obsolete("Use Rotate(Vector3 eulerAngle,Vector2? pivotPercentage = null) instead.")]
        public void Rotate(float angle, Vector2? pivotPercentage = null)
        {
            var pivot = pivotPercentage.HasValue
                ? new Vector3(
                    Mathf.Lerp(Vertices[0].x, Vertices[2].x, pivotPercentage.Value.x),
                    Mathf.Lerp(Vertices[0].y, Vertices[2].y, pivotPercentage.Value.y),
                    0
                )
                : GetComponentPosition();
            var rotation = Quaternion.Euler(0, 0, angle);
            for (var i = 0; i < Vertices.Length; i++) Vertices[i] = pivot + rotation * (Vertices[i] - pivot);

            _runtimeAngle.z += angle;
            _rotation *= rotation;
        }

        /// <summary>
        ///     旋转字符
        /// </summary>
        /// <remarks>
        ///     Rotate mesh.
        /// </remarks>
        /// <param name="eulerAngle">旋转角度。</param>
        /// <param name="pivotPercentage">可选的旋转枢轴点（0到1的比例）。</param>
        public void Rotate(Vector3 eulerAngle, Vector2? pivotPercentage = null)
        {
            Rotate(Quaternion.Euler(eulerAngle), pivotPercentage);
        }

        /// <summary>
        ///     旋转字符
        /// </summary>
        /// <remarks>
        ///     Rotate mesh.
        /// </remarks>
        /// >
        /// <param name="rotation">旋转</param>
        /// <param name="pivotPercentage">可选的旋转枢轴点（0到1的比例）。</param>
        public void Rotate(Quaternion rotation, Vector2? pivotPercentage = null)
        {
            var pivot = pivotPercentage.HasValue
                ? new Vector3(
                    Mathf.Lerp(Vertices[0].x, Vertices[2].x, pivotPercentage.Value.x),
                    Mathf.Lerp(Vertices[0].y, Vertices[2].y, pivotPercentage.Value.y),
                    0
                )
                : GetComponentPosition();
            for (var i = 0; i < Vertices.Length; i++) Vertices[i] = pivot + rotation * (Vertices[i] - pivot);

            _runtimeAngle += rotation.eulerAngles;
            _rotation *= rotation;
        }

        /// <summary>
        ///     初始化 Transform 数据。
        /// </summary>
        /// <remarks>
        ///     Initializes transform data with default values.
        /// </remarks>
        public void Reset()
        {
            Array.Copy(OriginalVertices, 0, Vertices, 0, 4);

            _runtimeLocalPosition = Vector3.zero;
            _runtimeAngle = Vector3.zero;
            _runtimeRotation = Quaternion.identity;
            _runtimeScale = new Vector2(1, 1);
        }


        /// <summary>
        ///     记录当前 Transform 数据。
        /// </summary>
        /// <remarks>
        ///     Records current transform data for later use.
        /// </remarks>
        public void Record()
        {
            _localPosition = _runtimeLocalPosition;
            _eulerAngle = _runtimeAngle;
            _localScale = _runtimeScale;
            _rotation = _runtimeRotation;
        }

        /// <summary>
        ///     计算本地坐标中心点。
        /// </summary>
        /// <remarks>
        ///     Calculates the center position of local coordinates.
        /// </remarks>
        /// <returns>本地坐标中心点</returns>
        private Vector3 GetComponentPosition()
        {
            var center = Vector3.zero;
            foreach (var vertex in Vertices) center += vertex;

            center /= Vertices.Length;
            return center;
        }


        /// <summary>
        ///     获取屏幕坐标位置。
        /// </summary>
        /// <remarks>
        ///     Gets the screen position by combining local and component positions.
        /// </remarks>
        /// <returns>屏幕坐标位置</returns>
        private Vector3 GetScreenPosition()
        {
            return GetComponentPosition() + LocalPosition + TMPComponent.transform.position;
        }

        /// <summary>
        ///     设置屏幕坐标位置。
        /// </summary>
        /// <remarks>
        ///     Set the screen position.
        /// </remarks>
        /// <returns>屏幕坐标位置</returns>
        private void SetScreenPosition(Vector3 screenPosition)
        {
            Move(TransformScreenToLocal(screenPosition));
        }

        /// <summary>
        ///     将屏幕坐标转换为本地坐标。
        /// </summary>
        /// <remarks>
        ///     Converts screen coordinates to local coordinates.
        /// </remarks>
        /// <param name="screenPos">屏幕坐标</param>
        /// <returns>本地坐标</returns>
        public Vector3 TransformScreenToLocal(Vector3 screenPos)
        {
            return screenPos - TMPComponent.transform.position - GetComponentPosition() + _runtimeLocalPosition;
        }
    }

    public struct CharacterColor
    {
        public Color32[] OriginalVerticesColor;
        public Color32[] VerticesColor;

        private Color _runtimeColor;
        private Color _color;

        public Color Color
        {
            get => _color;
            set => SetColor(value);
        }

        private float _runtimeAlpha;
        private float _alpha;

        public float Alpha
        {
            get => _alpha;
            set => SetAlpha(value);
        }


        /// <summary>
        ///     设置所有顶点的颜色。
        /// </summary>
        /// <remarks>
        ///     Sets color for all vertices using Color struct.
        /// </remarks>
        /// <param name="color">颜色</param>
        public void SetColor(Color color)
        {
            Array.Fill(VerticesColor, color);
            _runtimeColor = color;
            _runtimeAlpha = color.a;
        }

        /// <summary>
        ///     设置顶点的透明度。
        /// </summary>
        /// <remarks>
        ///     Sets alpha value for all vertices (0 to 1 range).
        /// </remarks>
        /// <param name="alpha">透明度，范围为 0 到 1</param>
        public void SetAlpha(float alpha)
        {
            var alpha32 = (int)(alpha * 255);
            alpha32 = Mathf.Clamp(alpha32, 0, 255);
            for (var i = 0; i < VerticesColor.Length; i++) VerticesColor[i].a = Convert.ToByte(alpha32);

            _runtimeColor.a = alpha;
            _runtimeAlpha = alpha;
        }

        public void Reset()
        {
            Array.Copy(OriginalVerticesColor, 0, VerticesColor, 0, 4);
            _runtimeColor = OriginalVerticesColor[0];
            _runtimeAlpha = OriginalVerticesColor[0].a;
        }

        public void Record()
        {
            _color = _runtimeColor;
            _alpha = _runtimeAlpha;
        }
    }

    public struct CharacterTime
    {
        public float Time;
        public float DeltaTime;

        public void Reset()
        {
            Time = 0;
            DeltaTime = 0;
        }
    }

    /// <summary>
    ///     描述一个字符的结构体，包含字符数据、颜色、顶点信息等。
    /// </summary>
    /// <remarks>
    ///     A structure that describes a character, including character data, color, vertex information, etc.
    /// </remarks>
    public class Character
    {
        public TMP_CharacterInfo CharacterInfo;

        // Character data.
        public int CharIndex;
        public CharacterColor Color;
        public int EndIndex;
        public TMP_MeshInfo MeshInfo;
        public int StartIndex;
        public CharacterTime Time;
        public TMP_Text TMPComponent;
        public int TotalCount;
        public Transform Transform;
        public bool Visible;

        public void Update()
        {
            if (!Visible) return;

            Transform.Record();
            Color.Record();
            for (var k = 0; k < Transform.Vertices.Length; k++)
            {
                var index = StartIndex + k;
                MeshInfo.vertices[index] = Transform.Vertices[k];
                MeshInfo.colors32[index] = Color.VerticesColor[k];
            }
        }

        public void Reset()
        {
            Transform.Reset();
            Color.Reset();
            Time.Reset();
        }
    }
}