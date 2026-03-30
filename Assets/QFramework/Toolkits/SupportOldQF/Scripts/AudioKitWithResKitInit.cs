using System;
using UnityEngine;

namespace QFramework
{
    public class AudioKitWithResKitInit
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            AudioKit.Config.AudioLoaderPool = new ResKitAudioLoaderPool();
        }
    }

    public class ResKitAudioLoaderPool : AbstractAudioLoaderPool
    {
        protected override IAudioLoader CreateLoader()
        {
            return new ResKitAudioLoader();
        }

        public class ResKitAudioLoader : IAudioLoader
        {
            private ResLoader mResLoader;

            public AudioClip Clip { get; private set; }

            public AudioClip LoadClip(AudioSearchKeys audioSearchKeys)
            {
                if (mResLoader == null) mResLoader = ResLoader.Allocate();

                Clip = mResLoader.LoadSync<AudioClip>(audioSearchKeys.AssetName);

                return Clip;
            }

            public void LoadClipAsync(AudioSearchKeys audioSearchKeys, Action<bool, AudioClip> onLoad)
            {
                if (mResLoader == null) mResLoader = ResLoader.Allocate();

                mResLoader.Add2Load<AudioClip>(audioSearchKeys.AssetName, (b, res) =>
                {
                    Clip = res.Asset as AudioClip;
                    onLoad(b, res.Asset as AudioClip);
                });

                mResLoader.LoadAsync();
            }

            public void Unload()
            {
                Clip = null;
                mResLoader?.Recycle2Cache();
                mResLoader = null;
            }
        }
    }
}