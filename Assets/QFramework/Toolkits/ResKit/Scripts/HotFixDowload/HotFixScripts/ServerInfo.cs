using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace QFramework
{
    [Serializable]
    public class ServerInfo
    {
        [XmlElement("GameVersion")] public VersionInfo[] GameVersion;
    }

    //当前游戏版本对应的所有补丁
    [Serializable]
    public class VersionInfo
    {
        [XmlAttribute] public string Version;

        [XmlElement] public Pathces[] Pathces;
    }

    //一个总补丁包
    [Serializable]
    public class Pathces
    {
        [XmlAttribute] public int Version;

        [XmlAttribute] public string Des;

        [XmlElement] public List<Patch> Files;
    }

    /// <summary>
    ///     单个补丁包
    /// </summary>
    [Serializable]
    public class Patch
    {
        [XmlAttribute] public string Name;

        [XmlAttribute] public string Url;

        [XmlAttribute] public string Platform;

        [XmlAttribute] public string Md5;

        [XmlAttribute] public float Size;
    }
}