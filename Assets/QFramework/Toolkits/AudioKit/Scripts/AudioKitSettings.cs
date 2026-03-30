/****************************************************************************
 * Copyright (c) 2016 ~ 2024 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    /// <summary>
    ///     专门用来为音频做设置
    /// </summary>
    public class AudioKitSettings
    {
        // 用来存储的Key
        private const string KEY_AUDIO_MANAGER_SOUND_ON = "KEY_AUDIO_MANAGER_SOUND_ON";

        private const string KEY_AUDIO_MANAGER_MUSIC_ON = "KEY_AUDIO_MANAGER_MUSIC_ON";
        private const string KEY_AUDIO_MANAGER_VOICE_ON = "KEY_AUDIO_MANAGER_VOICE_ON";

        private const string KEY_AUDIO_MANAGER_VOICE_VOLUME = "KEY_AUDIO_MANAGER_VOICE_VOLUME";
        private const string KEY_AUDIO_MANAGER_SOUND_VOLUME = "KEY_AUDIO_MANAGER_SOUND_VOLUME";
        private const string KEY_AUDIO_MANAGER_MUSIC_VOLUME = "KEY_AUDIO_MANAGER_MUSIC_VOLUME";

        public AudioKitSettings()
        {
            IsSoundOn = new PlayerPrefsBooleanProperty(KEY_AUDIO_MANAGER_SOUND_ON, true);

            IsMusicOn = new PlayerPrefsBooleanProperty(KEY_AUDIO_MANAGER_MUSIC_ON, true);

            IsVoiceOn = new PlayerPrefsBooleanProperty(KEY_AUDIO_MANAGER_VOICE_ON, true);


            IsOn = new CustomProperty<bool>(
                () => IsSoundOn.Value && IsMusicOn.Value && IsVoiceOn.Value,
                isOn =>
                {
                    Debug.Log(isOn);
                    IsSoundOn.Value = isOn;
                    IsMusicOn.Value = isOn;
                    IsVoiceOn.Value = isOn;
                }
            );

            SoundVolume = new PlayerPrefsFloatProperty(KEY_AUDIO_MANAGER_SOUND_VOLUME, 1.0f);

            MusicVolume = new PlayerPrefsFloatProperty(KEY_AUDIO_MANAGER_MUSIC_VOLUME, 1.0f);

            VoiceVolume = new PlayerPrefsFloatProperty(KEY_AUDIO_MANAGER_VOICE_VOLUME, 1.0f);
        }

        public PlayerPrefsBooleanProperty IsSoundOn { get; }

        public PlayerPrefsBooleanProperty IsMusicOn { get; }

        public PlayerPrefsBooleanProperty IsVoiceOn { get; }

        public PlayerPrefsFloatProperty SoundVolume { get; private set; }

        public PlayerPrefsFloatProperty MusicVolume { get; private set; }

        public PlayerPrefsFloatProperty VoiceVolume { get; private set; }

        public CustomProperty<bool> IsOn { get; private set; }
    }
}