using System.Collections.Generic;

namespace QFramework
{
    public class PanelCodeInfo
    {
        public readonly List<BindInfo> BindInfos = new();
        public readonly List<ElementCodeInfo> ElementCodeDatas = new();
        public Dictionary<string, string> DicNameToFullName = new();
        public string GameObjectName;

        public string Identifier { get; set; }
        public bool Changed { get; set; }
        public IEnumerable<string> ForeignKeys { get; private set; }
    }
}