using UnityEngine;

namespace QFramework
{
    public class FluentSoundAPI : IPoolable, IPoolType
    {
        private AudioClip mClip;
        private bool mLoop;

        private string mName;
        private float mPitch = 1;
        private float mVolumeScale = 1;

        public void OnRecycled()
        {
            mName = null;
            mLoop = false;
        }

        public bool IsRecycled { get; set; }

        public void Recycle2Cache()
        {
            SafeObjectPool<FluentSoundAPI>.Instance.Recycle(this);
        }

        public static FluentSoundAPI Allocate()
        {
            return SafeObjectPool<FluentSoundAPI>.Instance.Allocate();
        }

        public FluentSoundAPI WithName(string name)
        {
            mName = name;
            return this;
        }

        public FluentSoundAPI WithAudioClip(AudioClip clip)
        {
            mClip = clip;
            return this;
        }

        public FluentSoundAPI Loop(bool loop)
        {
            mLoop = loop;
            return this;
        }

        public FluentSoundAPI VolumeScale(float volumeScale)
        {
            mVolumeScale = volumeScale;
            return this;
        }

        public FluentSoundAPI Pitch(float pitch)
        {
            mPitch = pitch;
            return this;
        }


        public AudioPlayer Play()
        {
            AudioPlayer soundPlayer = null;
            if (mName != null)
                soundPlayer = AudioKit.PlaySound(mName, mLoop, p => { Recycle2Cache(); }, mVolumeScale, mPitch);
            else
                soundPlayer = AudioKit.PlaySound(mClip, mLoop, p => { Recycle2Cache(); }, mVolumeScale, mPitch);

            if (soundPlayer == null) Recycle2Cache();

            return soundPlayer;
        }
    }
}