/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2018.5 liangxie
 *
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
    public class Res : SimpleRC, IRes, IPoolable, IPoolType
    {
        protected Object mAsset;

        protected string mAssetName;
        private ResState mResState = ResState.Waiting;

        protected Res(string assetName)
        {
            IsRecycled = false;
            mAssetName = assetName;
        }

        public Res()
        {
            IsRecycled = false;
        }

        public bool IsRecycled { get; set; }

        public string AssetName
        {
            get => mAssetName;
            protected set => mAssetName = value;
        }


        public ResState State
        {
            get => mResState;
            set
            {
                mResState = value;
                if (mResState == ResState.Ready) NotifyResLoadDoneEvent(true);
            }
        }

        public virtual string OwnerBundleName { get; set; }

        public Type AssetType { get; set; }

        /// <summary>
        ///     弃用
        /// </summary>
        public float Progress
        {
            get
            {
                switch (mResState)
                {
                    case ResState.Loading:
                        return CalculateProgress();
                    case ResState.Ready:
                        return 1;
                }

                return 0;
            }
        }

        public Object Asset => mAsset;


        public void RegisteOnResLoadDoneEvent(Action<bool, IRes> listener)
        {
            if (listener == null) return;

            if (mResState == ResState.Ready)
            {
                listener(true, this);
                return;
            }

            mOnResLoadDoneEvent += listener;
        }

        public void UnRegisteOnResLoadDoneEvent(Action<bool, IRes> listener)
        {
            if (listener == null) return;

            if (mOnResLoadDoneEvent == null) return;

            mOnResLoadDoneEvent -= listener;
        }

        private event Action<bool, IRes> mOnResLoadDoneEvent;

        protected virtual float CalculateProgress()
        {
            return 0;
        }

        protected void OnResLoadFaild()
        {
            mResState = ResState.Waiting;
            NotifyResLoadDoneEvent(false);
        }

        private void NotifyResLoadDoneEvent(bool result)
        {
            if (mOnResLoadDoneEvent != null)
            {
                mOnResLoadDoneEvent(result, this);
                mOnResLoadDoneEvent = null;
            }
        }

        protected bool CheckLoadAble()
        {
            return mResState == ResState.Waiting;
        }

        protected void HoldDependRes()
        {
            var depends = GetDependResList();
            if (depends == null || depends.Length == 0) return;

            for (var i = depends.Length - 1; i >= 0; --i)
            {
                var resSearchRule = ResSearchKeys.Allocate(depends[i], null, typeof(AssetBundle));
                var res = ResMgr.Instance.GetRes(resSearchRule);
                resSearchRule.Recycle2Cache();

                if (res != null) res.Retain();
            }
        }

        protected void UnHoldDependRes()
        {
            var depends = GetDependResList();
            if (depends == null || depends.Length == 0) return;

            for (var i = depends.Length - 1; i >= 0; --i)
            {
                var resSearchRule = ResSearchKeys.Allocate(depends[i]);
                var res = ResMgr.Instance.GetRes(resSearchRule);
                resSearchRule.Recycle2Cache();

                if (res != null) res.Release();
            }
        }

        #region 子类实现

        public virtual bool LoadSync()
        {
            return false;
        }

        public virtual void LoadAsync()
        {
        }

        public virtual string[] GetDependResList()
        {
            return null;
        }

        public bool IsDependResLoadFinish()
        {
            var depends = GetDependResList();
            if (depends == null || depends.Length == 0) return true;

            for (var i = depends.Length - 1; i >= 0; --i)
            {
                var resSearchRule = ResSearchKeys.Allocate(depends[i]);
                var res = ResMgr.Instance.GetRes(resSearchRule);
                resSearchRule.Recycle2Cache();

                if (res == null || res.State != ResState.Ready) return false;
            }

            return true;
        }

        public virtual bool UnloadImage(bool flag)
        {
            return false;
        }

        public bool ReleaseRes()
        {
            if (mResState == ResState.Loading) return false;

            if (mResState != ResState.Ready) return true;

            //Log.I("Release Res:" + mName);

            OnReleaseRes();

            mResState = ResState.Waiting;
            mOnResLoadDoneEvent = null;
            return true;
        }

        protected virtual void OnReleaseRes()
        {
            //如果Image 直接释放了，这里会直接变成NULL
            if (mAsset != null)
            {
                ResUnloadHelper.UnloadRes(mAsset);

                mAsset = null;
            }
        }

        protected override void OnZeroRef()
        {
            if (mResState == ResState.Loading) return;

            ReleaseRes();
        }

        public virtual void Recycle2Cache()
        {
        }

        public virtual void OnRecycled()
        {
            mAssetName = null;
            mOnResLoadDoneEvent = null;
        }

        public virtual IEnumerator DoLoadAsync(Action finishCallback)
        {
            finishCallback();
            yield break;
        }

        public override string ToString()
        {
            return string.Format("Name:{0}\t State:{1}\t RefCount:{2}", AssetName, State, RefCount);
        }

        #endregion
    }
}