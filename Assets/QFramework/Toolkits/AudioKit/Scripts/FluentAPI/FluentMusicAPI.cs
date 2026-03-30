using UnityEngine;

namespace QFramework
{
    public class FluentMusicAPI : IPoolable, IPoolType
    {
        private AudioClip mClip;
        private bool mLoop = true;


        private string mName;
        private float mVolumeScale = 1;

        public void OnRecycled()
        {
            mName = null;
            mClip = null;
            mLoop = true;
            mVolumeScale = 1;
        }

        public bool IsRecycled { get; set; }

        public void Recycle2Cache()
        {
            SafeObjectPool<FluentMusicAPI>.Instance.Recycle(this);
        }

        public static FluentMusicAPI Allocate()
        {
            return SafeObjectPool<FluentMusicAPI>.Instance.Allocate();
        }

        public FluentMusicAPI WithName(string name)
        {
            mName = name;
            return this;
        }

        public FluentMusicAPI WithAudioClip(AudioClip clip)
        {
            mClip = clip;
            return this;
        }

        public FluentMusicAPI Loop(bool loop)
        {
            mLoop = loop;
            return this;
        }

        public FluentMusicAPI VolumeScale(float volumeScale)
        {
            mVolumeScale = volumeScale;
            return this;
        }

        public void Play()
        {
            if (mName != null)
                AudioKit.PlayMusic(mName, mLoop, onEndCallback: Recycle2Cache, volume: mVolumeScale);
            else
                AudioKit.PlayMusic(mClip, mLoop, onEndCallback: Recycle2Cache, volume: mVolumeScale);
        }
    }
}