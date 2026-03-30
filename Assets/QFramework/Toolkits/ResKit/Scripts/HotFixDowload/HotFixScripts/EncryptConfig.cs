using System;

namespace QFramework
{
    [Serializable]
    public class EncryptConfig
    {
        public bool EncryptAB;
        public bool EncryptKey;
        public string AESKey = "QFramework";
    }
}