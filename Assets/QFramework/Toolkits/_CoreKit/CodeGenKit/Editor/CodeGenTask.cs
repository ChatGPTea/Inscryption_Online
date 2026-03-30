/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public enum CodeGenTaskStatus
    {
        Search,
        Gen,
        Compile,
        Complete
    }

    public enum GameObjectFrom
    {
        Scene,
        Prefab
    }

    [Serializable]
    public class CodeGenTask
    {
        public bool ShowLog;

        // state
        public CodeGenTaskStatus Status;

        // input
        public GameObject GameObject;
        public GameObjectFrom From = GameObjectFrom.Scene;


        // search
        public List<StringPair> NameToFullName = new();
        public List<BindInfo> BindInfos = new();

        // info
        public string ScriptsFolder;
        public string ClassName;
        public string Namespace;

        // result
        public string MainCode;
        public string DesignerCode;
    }

    [Serializable]
    public class StringPair
    {
        public string Key;
        public string Value;

        public StringPair(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
#endif