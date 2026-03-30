/****************************************************************************
 * Copyright (c) 2015 ~ 2024 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class AssetDataGroup
    {
        private List<ABUnit> mABUnitArray;
        private Dictionary<string, AssetData> mAssetDataMap;
        private Dictionary<string, AssetData> mUUID4AssetData;

        public AssetDataGroup(string key)
        {
            this.key = key;
        }

        public AssetDataGroup(SerializeData data)
        {
            key = data.key;
            SetSerializeData(data);
        }

        public IEnumerable<AssetData> AssetDatas => mAssetDataMap.Values;
        public IEnumerable<ABUnit> AssetBundleDatas => mABUnitArray;
        public string key { get; }

        public void Reset()
        {
            mABUnitArray?.Clear();

            mAssetDataMap?.Clear();
        }

        public int AddAssetBundleName(string name, string[] depends)
        {
            if (string.IsNullOrEmpty(name)) return -1;

            if (mABUnitArray == null) mABUnitArray = new List<ABUnit>();

            var resSearchRule = ResSearchKeys.Allocate(name);
            var config = GetAssetData(resSearchRule);
            resSearchRule.Recycle2Cache();

            if (config != null) return config.AssetBundleIndex;

            mABUnitArray.Add(new ABUnit(name, depends));

            var index = mABUnitArray.Count - 1;

            AddAssetData(new AssetData(name, ResLoadType.AssetBundle, index, null));

            return index;
        }

        public bool GetAssetBundleName(string assetName, int index, out string result)
        {
            result = null;

            if (mABUnitArray == null) return false;

            if (index >= mABUnitArray.Count) return false;

            if (mAssetDataMap.ContainsKey(assetName))
            {
                result = mABUnitArray[index].abName;
                return true;
            }

            return false;
        }

        public ABUnit GetABUnit(string assetName)
        {
            var resSearchRule = ResSearchKeys.Allocate(assetName);

            var data = GetAssetData(resSearchRule);

            resSearchRule.Recycle2Cache();

            if (data == null) return null;

            if (mABUnitArray == null) return null;

            return mABUnitArray[data.AssetBundleIndex];
        }

        public bool GetAssetBundleDepends(string abName, out string[] result)
        {
            result = null;

            var unit = GetABUnit(abName);

            if (unit == null) return false;

            result = unit.abDepends;

            return true;
        }

        public AssetData GetAssetData(ResSearchKeys resSearchRule)
        {
            AssetData result = null;

            if (resSearchRule.OwnerBundle != null && mUUID4AssetData != null)
                return mUUID4AssetData.TryGetValue(resSearchRule.OwnerBundle + resSearchRule.AssetName, out result)
                    ? result
                    : null;

            if (resSearchRule.OwnerBundle == null && mAssetDataMap != null)
                return mAssetDataMap.TryGetValue(resSearchRule.AssetName, out result) ? result : null;

            return result;
        }

        public bool AddAssetData(AssetData data)
        {
            if (mAssetDataMap == null) mAssetDataMap = new Dictionary<string, AssetData>();

            if (mUUID4AssetData == null) mUUID4AssetData = new Dictionary<string, AssetData>();

            var key = data.AssetName.ToLower();

            if (mAssetDataMap.ContainsKey(key))
            {
                var resSearchRule = ResSearchKeys.Allocate(data.AssetName);
                var old = GetAssetData(resSearchRule);
                resSearchRule.Recycle2Cache();

                try
                {
                    Debug.LogFormat("Already Add AssetData :{0} \n OldAB:{1}      NewAB:{2}", data.AssetName,
                        mABUnitArray[old.AssetBundleIndex].abName, mABUnitArray[data.AssetBundleIndex].abName);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            }
            else
            {
                mAssetDataMap.Add(key, data);
            }

            if (mUUID4AssetData.ContainsKey(data.UUID))
            {
                var resSearchRule = ResSearchKeys.Allocate(data.AssetName, data.OwnerBundleName);

                var old = GetAssetData(resSearchRule);
                resSearchRule.Recycle2Cache();

                Debug.LogWarningFormat("Already Add AssetData :{0} \n OldAB:{1}      NewAB:{2}", data.UUID,
                    mABUnitArray[old.AssetBundleIndex].abName, mABUnitArray[data.AssetBundleIndex].abName);
            }
            else
            {
                mUUID4AssetData.Add(data.UUID, data);
            }

            return true;
        }

        public SerializeData GetSerializeData()
        {
            var sd = new SerializeData();
            sd.key = key;
            sd.abUnitArray = mABUnitArray.ToArray();
            if (mAssetDataMap != null)
            {
                var acArray = new AssetData[mAssetDataMap.Count];

                var index = 0;
                foreach (var item in mAssetDataMap) acArray[index++] = item.Value;

                sd.assetDataArray = acArray;
            }

            return sd;
        }


        private void SetSerializeData(SerializeData data)
        {
            if (data == null) return;

            mABUnitArray = new List<ABUnit>(data.abUnitArray);

            if (data.assetDataArray != null)
            {
                mAssetDataMap = new Dictionary<string, AssetData>();

                foreach (var config in data.assetDataArray) AddAssetData(config);
            }
        }

        /// <summary>
        ///     代表依赖关系的类
        /// </summary>
        [Serializable]
        public class ABUnit
        {
            public string abName;
            public string[] abDepends;
            public string Hash;

            public ABUnit(string name, string[] depends)
            {
                abName = name;
                if (depends == null || depends.Length == 0)
                {
                }
                else
                {
                    abDepends = depends;
                }
            }

            public override string ToString()
            {
                var result = string.Format("ABName:" + abName);
                if (abDepends == null) return result;

                foreach (var abDepend in abDepends) result += string.Format(" #:{0}", abDepend);

                return result;
            }
        }

        [Serializable]
        public class SerializeData
        {
            private ABUnit[] mAbUnitArray;
            private AssetData[] mAssetDataArray;
            private string mKey;

            public string key
            {
                get => mKey;
                set => mKey = value;
            }

            public ABUnit[] abUnitArray
            {
                get => mAbUnitArray;
                set => mAbUnitArray = value;
            }

            public AssetData[] assetDataArray
            {
                get => mAssetDataArray;
                set => mAssetDataArray = value;
            }
        }
    }
}