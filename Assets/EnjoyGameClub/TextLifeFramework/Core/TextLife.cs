/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnjoyGameClub.TextLifeFramework.Core.Animation;
using EnjoyGameClub.TextLifeFramework.Core.Match;
using EnjoyGameClub.TextLifeFramework.Core.SerializedObject;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EnjoyGameClub.TextLifeFramework.Core
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    [ExecuteInEditMode]
    [Serializable]
    public partial class TextLife : UIBehaviour
    {
        #region Public Properties

        /// <summary>
        ///     已注册的动画流程类型字典（标签与类型映射）
        /// </summary>
        /// <remarks>
        ///     Dictionary of registered animation process types (tag -> type mapping)
        /// </remarks>
        public static Dictionary<string, Type> RegisteredProcessTypes { get; private set; } =
            new();

        /// <summary>
        ///     富文本捕获配置
        /// </summary>
        /// <remarks>
        ///     Rich text match config.
        /// </remarks>
        /// >
        public MatchConfig MatchConfig;

        /// <summary>
        ///     是否开启调试模式
        /// </summary>
        /// <remarks>
        ///     Switch debug mode.
        /// </remarks>
        /// >
        public bool Debug;

        /// <summary>
        ///     是否运行TimeScale影响TimeLife
        /// </summary>
        public bool DisableTimeScale;

        /// <summary>
        ///     原始文本内容（支持文本区域编辑）
        /// </summary>
        /// <remarks>
        ///     Original text content with text area editing support
        /// </remarks>
        [TextArea(3, 10)] public string Text;

        /// <summary>
        ///     匹配到的动画流程列表
        /// </summary>
        /// <remarks>
        ///     List of matched animation processes
        /// </remarks>
        public SerializedObjectList<AnimationProcess> MatchProcesses = new();

        /// <summary>
        ///     全局应用的动画流程列表
        /// </summary>
        /// <remarks>
        ///     List of globally applied animation processes
        /// </remarks>
        public SerializedObjectList<AnimationProcess> GlobalProcesses = new();

        #endregion

        #region Private Fields

        /// <summary>
        ///     持久化存储的动画流程列表
        /// </summary>
        /// <remarks>
        ///     List of persisted animation processes
        /// </remarks>
        [SerializeField] private SerializedDictionary<AnimationProcess, AnimationProcess> _persistedProcessesDict =
            new();

        private TMP_Text _tempText;
        private List<MatchData> _matchedProcessDataList = new();
        private Character[] _characters = new Character[1];

        private float _animationTime;

        private string _originalText;
        private string _runtimeText;
        private string _tmpFormattedText;
        private string _processedText;
        private int _globalProcessesCount;

        private Dictionary<string, List<AnimationProcess>> _activeProcessMap = new();

        #endregion


        #region Unity Lifecycle

        protected override void Awake()
        {
#if UNITY_EDITOR
            MatchConfig ??= DefaultMatchConfig.Instance.MatchConfig;
#endif
        }

        protected override void Start()
        {
            InitTMPTextComponent();
            InitAnimation();
            InitStateMachine();
            CleanProcesses();
            RefreshActiveProcesses();
        }

        private void LateUpdate()
        {
            stateMachine?.LateUpdateState();
            if (_tempText.text == "")
            {
                _tempText.ForceMeshUpdate();
                BuildCharacterData();
            }
        }

        private void Update()
        {
            stateMachine?.UpdateState();
        }

        #endregion


        #region Public Methods

        /// <summary>
        ///     通过泛型类型获取动画流程列表
        /// </summary>
        /// <typeparam name="T">动画类型</typeparam>
        /// <returns></returns>
        public List<T> GetMatchProcesses<T>()
        {
            return _activeProcessMap.Values.SelectMany(x => x).OfType<T>().ToList();
        }


        /// <summary>
        ///     通过泛型类型获取全局动画流程列表
        /// </summary>
        /// <typeparam name="T">动画流程类型</typeparam>
        /// <returns>指定类型的全局动画流程列表</returns>
        /// <remarks>
        ///     Get global animation processes by generic type
        /// </remarks>
        public List<T> GetGlobalProcesses<T>()
        {
            return GlobalProcesses.List.Select(x => x).OfType<T>().ToList();
        }

        /// <summary>
        ///     通过匹配标签获取关联的动画流程列表
        /// </summary>
        /// <param name="matchTag">匹配标签</param>
        /// <returns>关联的动画流程列表（不存在时返回null）</returns>
        /// <remarks>
        ///     Get animation processes by match tag
        /// </remarks>
        public List<AnimationProcess> GetMatchProcessesByMatchTag(string matchTag)
        {
            return _activeProcessMap.GetValueOrDefault(matchTag);
        }

        /// <summary>
        ///     通过名称获取全局动画流程列表
        /// </summary>
        /// <param name="scriptableName">匹配名称的全局动画流程列表</param>
        /// <returns></returns>
        public List<AnimationProcess> GetGlobalProcessesByName(string scriptableName)
        {
            var list = new List<AnimationProcess>();
            list.AddRange(GlobalProcesses.List.Where(x => x.name == scriptableName).ToList());
            return list;
        }

        /// <summary>
        ///     通过名称和类型获取全局动画流程列表
        /// </summary>
        /// <param name="scriptableName">可脚本化对象名称</param>
        /// <returns>匹配名称的全局动画流程列表</returns>
        /// <remarks>
        ///     Get global animation processes by name
        /// </remarks>
        public List<T> GetGlobalProcessesByName<T>(string scriptableName) where T : AnimationProcess
        {
            var list = GlobalProcesses.List.Where(x =>
                    x.name == scriptableName && (typeof(T) == x.GetType() || typeof(T).IsAssignableFrom(x.GetType())))
                ;
            return list.Select(animationProcess => animationProcess as T).ToList();
        }

        /// <summary>
        ///     通过名称获取单个全局动画流程
        /// </summary>
        /// <param name="scriptableName">可脚本化对象名称</param>
        /// <returns>匹配名称的全局动画流程（不存在时返回null）</returns>
        /// <remarks>
        ///     Get single global animation process by name
        /// </remarks>
        public AnimationProcess GetGlobalProcessByName(string scriptableName)
        {
            return GlobalProcesses.List.FirstOrDefault(x => x.name == scriptableName);
        }

        /// <summary>
        ///     通过名称和类型获取全局动画流程列表
        /// </summary>
        /// <typeparam name="T">动画流程类型</typeparam>
        /// <param name="scriptableName">可脚本化对象名称</param>
        /// <returns>匹配名称和类型的全局动画流程列表</returns>
        /// <remarks>
        ///     Get typed global animation processes by name
        /// </remarks>
        public T GetGlobalProcessByName<T>(string scriptableName) where T : AnimationProcess
        {
            return GlobalProcesses.List.FirstOrDefault(x => x.name == scriptableName) as T;
        }


        /// <summary>
        ///     通过名称获取匹配的动画流程列表
        /// </summary>
        /// <param name="scriptableName">可脚本化对象名称</param>
        /// <returns>匹配名称的动画流程列表（不存在时返回空列表）</returns>
        /// <remarks>
        ///     Get match animation processes by scriptable object name
        /// </remarks>
        public List<AnimationProcess> GetMatchProcessesByName(string scriptableName)
        {
            var list = new List<AnimationProcess>();
            list.AddRange(_persistedProcessesDict.Values.Where(x => x.name == scriptableName).ToList());
            return list;
        }

        /// <summary>
        ///     通过名称和类型获取单个匹配的动画流程
        /// </summary>
        /// <typeparam name="T">动画流程类型</typeparam>
        /// <param name="scriptableName">可脚本化对象名称</param>
        /// <returns>匹配名称和类型的全局动画流程（不存在时返回null）</returns>
        /// <remarks>
        ///     Get typed global animation process by name
        /// </remarks>
        public List<T> GetMatchProcessesByName<T>(string scriptableName) where T : AnimationProcess
        {
            var list = _persistedProcessesDict.Values.Where(x =>
                x.name == scriptableName && (typeof(T) == x.GetType() || typeof(T).IsAssignableFrom(x.GetType())));
            return list.Select(animationProcess => animationProcess as T).ToList();
        }


        /// <summary>
        ///     通过匹配标签添加全局动画流程
        /// </summary>
        /// <param name="matchTag">要匹配的流程标签</param>
        /// <returns>克隆后的动画流程列表，未找到匹配标签时返回null</returns>
        /// <remarks>
        ///     Adds global animation processes by matching tag.
        ///     Clones the original processes to ensure independence.
        /// </remarks>
        public List<AnimationProcess> AddGlobalProcessesByTag(string matchTag)
        {
            // 获取需要克隆的AnimationProcess
            var list = MatchConfig.MatchList.FirstOrDefault(data => data.matchTag == matchTag);
            if (list == null) return null;

            // 克隆AnimationProcess
            var cloneAnimationProcesses = new List<AnimationProcess>();
            foreach (var scriptableObject in list.Process)
            {
                var process = (AnimationProcess)scriptableObject;
                var cloneProcess = (AnimationProcess)process.Clone();
                cloneProcess.name = process.name;
                cloneAnimationProcesses.Add(cloneProcess);
            }

            GlobalProcesses.List.AddRange(cloneAnimationProcesses);
            return cloneAnimationProcesses;
        }

        /// <summary>
        ///     初始化动画时间线
        /// </summary>
        /// <remarks>
        ///     Init animation timeline
        /// </remarks>
        public void InitAnimation()
        {
            Parallel.ForEach(_persistedProcessesDict.Values, animationProcess => { animationProcess?.Start(); });
            Parallel.ForEach(GlobalProcesses.List, animationProcess => { animationProcess?.Start(); });
        }

        /// <summary>
        ///     重置动画时间线
        /// </summary>
        /// <remarks>
        ///     Reset animation timeline
        /// </remarks>
        public void ResetAnimation()
        {
            Parallel.ForEach(_persistedProcessesDict.Values, animationProcess => { animationProcess?.Reset(); });
            Parallel.ForEach(GlobalProcesses.List, animationProcess => { animationProcess?.Reset(); });
            Parallel.For(0, _characters.Length, i => { _characters[i]?.Reset(); });
            _animationTime = 0;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     记录动画的累计播放时间
        /// </summary>
        /// <remarks>
        ///     Records animation timeline progression.
        /// </remarks>
        private void RecordAnimationTime()
        {
            if (DisableTimeScale)
                _animationTime += Time.unscaledDeltaTime;
            else
                _animationTime += Time.deltaTime;
        }

        /// <summary>
        ///     初始化TMP文本组件并配置渲染回调
        /// </summary>
        /// <remarks>
        ///     Initializes the TextMeshPro component and configures rendering callbacks.
        /// </remarks>
        private void InitTMPTextComponent()
        {
            _tempText = GetComponent<TMP_Text>();
            _tempText.OnPreRenderText += _ => { BuildCharacterData(); };
        }

        /// <summary>
        ///     刷新当前激活的动画流程列表
        /// </summary>
        /// <remarks>
        ///     Updates active animation processes based on matched tags
        /// </remarks>
        private void RefreshActiveProcesses()
        {
            if (stateMachine.State.Name == "noneState") return;

            MatchProcesses.List.Clear();
            _activeProcessMap.Clear();
            var animationProcessesDict =
                new Dictionary<AnimationProcess, AnimationProcess>();
            // Load match processes.
            var activeTags = new HashSet<string>(_matchedProcessDataList.Select(x => x.MatchTag));
            foreach (var matchTag in activeTags)
            {
                var matchConfigData =
                    MatchConfig.MatchList.FirstOrDefault(configData => configData.matchTag == matchTag);
                var animationProcesses = new List<AnimationProcess>();
                if (matchConfigData == null) continue;

                foreach (var scriptableObject in matchConfigData.Process)
                {
                    var process = (AnimationProcess)scriptableObject;
                    if (animationProcessesDict.TryGetValue(process, out var existing))
                    {
                        animationProcesses.Add(existing);
                        continue;
                    }

                    if (_persistedProcessesDict.TryGetValue(process, out existing))
                    {
                        animationProcessesDict[process] = existing;
                        animationProcesses.Add(existing);
                        continue;
                    }


                    var cloneProcess = (AnimationProcess)process.Clone();
                    cloneProcess.name = process.name;

                    animationProcessesDict[process] = cloneProcess;
                    animationProcesses.Add(cloneProcess);
                    _persistedProcessesDict[process] = cloneProcess;
                }

                _activeProcessMap[matchTag] = animationProcesses;
            }

            MatchProcesses.List = animationProcessesDict.Values.ToList();
        }

        /// <summary>
        ///     清理无效的动画流程
        /// </summary>
        /// <remarks>
        ///     Cleans up invalid animation processes
        /// </remarks>
        private void CleanProcesses()
        {
            var invalidProcesses = _persistedProcessesDict.Where(kv => kv.Value == null).ToList();
            foreach (var keyValue in invalidProcesses) _persistedProcessesDict.Remove(keyValue);

            for (var i = GlobalProcesses.List.Count - 1; i >= 0; i--)
                if (GlobalProcesses.List[i] == null)
                    GlobalProcesses.List.RemoveAt(i);
        }

        /// <summary>
        ///     检查调试文本是否发生变化
        /// </summary>
        /// <remarks>
        ///     Checks if debug text has been modified
        /// </remarks>
        private bool HasDebugTextChanged()
        {
            return _runtimeText != Text;
        }

        /// <summary>
        ///     检查TMP文本是否发生变化
        /// </summary>
        /// <remarks>
        ///     Checks if TMP text has been modified
        /// </remarks>
        private bool HasTMPTextChanged()
        {
            return _tempText.text != _originalText &&
                   _tempText.text != _tmpFormattedText;
        }


        /// <summary>
        ///     开始匹配文本中的标签
        /// </summary>
        /// <remarks>
        ///     Starts matching tags in the text
        /// </remarks>
        /// <param name="originalString">原始文本</param>
        /// <returns>包含组件文本和处理文本的元组</returns>
        private (string toComponentString, string toProcessesString) StartMatchTag(string originalString)
        {
            // Prepare parse data
            _matchedProcessDataList.Clear();
            var overflowMode = _tempText.overflowMode;
            _tempText.overflowMode = TextOverflowModes.Overflow;
            _tempText.SetText(originalString);
            _tempText.ForceMeshUpdate();
            var processedText = _tempText.GetParsedText();
            _tempText.overflowMode = overflowMode;
            var tmpFormattedText = originalString;
            // Parser
            var processResult = TextMatcher.ParseRichText(processedText, MatchConfig);
            processedText = processResult.parseText;
            _matchedProcessDataList = processResult.matchDatas;
            tmpFormattedText = TextMatcher.RemoveTags(tmpFormattedText, _matchedProcessDataList);
            return (tmpFormattedText, processedText);
        }

        /// <summary>
        ///     重新解析文本中的标签并更新动画状态
        /// </summary>
        /// <remarks>
        ///     Re-parses text tags and updates animation states.
        /// </remarks>
        /// <param name="originalText">需要处理的原始文本/Raw text to process</param>
        private void ReMatchText(string originalText)
        {
            _runtimeText = originalText;
            var characterColor = _tempText.color;
            _tempText.color = new Color(0, 0, 0, 0);
            var str = StartMatchTag(_runtimeText);
            _tmpFormattedText = str.toComponentString;
            _processedText = str.toProcessesString;
            _tempText.SetText(_tmpFormattedText);
            _tempText.color = characterColor;
            _tempText.ForceMeshUpdate();
            RefreshActiveProcesses();
            BuildCharacterData();
            OnTextChanged();
        }

        /// <summary>
        ///     通知流程字符已发生改变
        /// </summary>
        /// <remarks>
        ///     Notify processes that characters have changed
        /// </remarks>
        /// >
        private void OnTextChanged()
        {
            Parallel.ForEach(_persistedProcessesDict.Values, animationProcess => { animationProcess?.ChangedText(); });
            Parallel.ForEach(GlobalProcesses.List, animationProcess => { animationProcess?.ChangedText(); });
        }

        /// <summary>
        ///     构建字符数据
        /// </summary>
        /// <remarks>
        ///     Builds character data for animation
        /// </remarks>
        private void BuildCharacterData()
        {
            var characterCount = _tempText.textInfo.characterCount;
            _characters = new Character[characterCount];
            Parallel.For(0, characterCount, i =>
            {
                var currentCharInfo = _tempText.textInfo.characterInfo[i];
                var currentCharMeshInfo = _tempText.textInfo.meshInfo[currentCharInfo.materialReferenceIndex];
                var startVertexIndex = currentCharInfo.vertexIndex;
                var nextVertexIndex = startVertexIndex + 4;
                var visible = currentCharInfo.isVisible;

                var originalVertices = new Vector3[4];
                var originalColor = new Color32[4];

                Array.Copy(currentCharMeshInfo.vertices, startVertexIndex, originalVertices, 0, 4);
                Array.Copy(currentCharMeshInfo.colors32, startVertexIndex, originalColor, 0, 4);
                // Build per-character data
                var data = new Character
                {
                    Transform = new Transform
                    {
                        OriginalVertices = originalVertices,
                        Vertices = new Vector3[4],
                        TMPComponent = _tempText
                    },
                    Color = new CharacterColor
                    {
                        OriginalVerticesColor = originalColor,
                        VerticesColor = new Color32[4]
                    },
                    CharIndex = i,
                    StartIndex = startVertexIndex,
                    EndIndex = nextVertexIndex,
                    MeshInfo = currentCharMeshInfo,
                    TotalCount = characterCount,
                    Visible = visible,
                    CharacterInfo = currentCharInfo,
                    TMPComponent = _tempText
                };

                // 初始化一次，记录某些数据。
                data.Reset();
                _characters[i] = data;
            });
        }

        /// <summary>
        ///     执行动画流程
        /// </summary>
        /// <remarks>
        ///     Executes animation processes
        /// </remarks>
        private void ExecuteProcess()
        {
            // Copy origin data.
            ResetCharacterData();
            // Match process.
            ExecuteMatchProcess();
            // Global Process.
            ExecuteGlobalProcess();
            // Update mesh.
            UpdateCharacterData();
        }

        /// <summary>
        ///     复制原始数据
        /// </summary>
        /// <remarks>
        ///     Copy origin data.
        /// </remarks>
        /// >
        private void ResetCharacterData()
        {
            Parallel.ForEach(_characters, character => { character.Reset(); });
        }


        /// <summary>
        ///     执行匹配动画流程
        /// </summary>
        /// <remarks>
        ///     Execute match process.
        /// </remarks>
        /// >
        private void ExecuteMatchProcess()
        {
            // Match process.
            foreach (var processData in _matchedProcessDataList)
            {
                if (!_activeProcessMap.TryGetValue(processData.MatchTag, out var processes))
                    continue;
                foreach (var process in processes)
                {
                    if (!process.Enable) continue;

                    var startIndex = processData.StartIndex;
                    var endIndex = processData.EndIndex;
                    var deltaTime = Time.deltaTime;

                    for (var i = startIndex; i < endIndex; i++)
                    {
                        if (i >= _characters.Length || !_characters[i].Visible) continue;

                        process.Progress(_animationTime, deltaTime, _characters[i]);
                    }
                }
            }
        }

        /// <summary>
        ///     执行全局流程
        /// </summary>
        /// <remarks>
        ///     Execute global processes.
        /// </remarks>
        /// >
        private void ExecuteGlobalProcess()
        {
            // Global process.
            foreach (var variableGlobalProcess in GlobalProcesses.List)
            {
                if (variableGlobalProcess == null || !variableGlobalProcess.Enable) continue;

                var deltaTime = Time.deltaTime;
                foreach (var character in _characters)
                {
                    if (!character.Visible) continue;

                    variableGlobalProcess.Progress(_animationTime, deltaTime, character);
                }
            }
        }

        /// <summary>
        ///     更新所有字符数据并刷新文本网格顶点
        /// </summary>
        /// <remarks>
        ///     Updates all character data and refreshes text mesh vertices.
        ///     Uses parallel processing for character updates followed by
        ///     a single vertex data update on the TextMeshPro component.
        /// </remarks>
        private void UpdateCharacterData()
        {
            Parallel.ForEach(_characters, character => { character.Update(); });
            _tempText.UpdateVertexData();
        }

        #endregion
    }
}