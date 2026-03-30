using UnityEngine;

namespace EnjoyGameClub.TextLifeFramework.Simple.Event
{
    public class AudioSourceEvent : ScriptableObject
    {
        private static GameObject AudioPlayer;
        public AudioClip Clip;

        public void PlayAudioSound()
        {
            if (AudioPlayer == null)
            {
                AudioPlayer = new GameObject("AudioPlayer");
                AudioPlayer.AddComponent<AudioSource>();
            }

            AudioPlayer.GetComponent<AudioSource>().PlayOneShot(Clip);
        }
    }
}