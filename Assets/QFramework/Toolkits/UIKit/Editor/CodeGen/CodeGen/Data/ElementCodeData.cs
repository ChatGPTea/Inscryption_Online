using System.Collections.Generic;

namespace QFramework
{
    public class ElementCodeInfo
    {
        public readonly List<BindInfo> BindInfos = new();
        public readonly List<ElementCodeInfo> ElementCodeDatas = new();
        public string BehaviourName;
        public BindInfo BindInfo;
        public Dictionary<string, string> DicNameToFullName = new();
    }
}