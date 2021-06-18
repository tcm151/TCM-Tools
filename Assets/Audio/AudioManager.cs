using System;
using System.Linq;
using UnityEngine;


namespace TCM.Audio
{
    public class AudioManager : MonoBehaviour
    {
        //- SINGLETON
        private static AudioManager instance;

        public static AudioManager Connect
        {
            get
            {
                if (!instance) Debug.LogError("No <color=red>AudioManager</color> in scene!");
                return instance;
            }
        }

        //- LOCAL VARIABLES
        [SerializeField] private int audioStreams = 4;
        [SerializeReference] private SFX[] soundEffects;
        [SerializeReference] private AudioSource[] source;

        //> INITIALIZATION
        private void Awake()
        {
            instance = this;
            if (source is { }) return;
            
            //- Add audio sources for multiple tracks
            for (int i = 0; i < audioStreams; i++) gameObject.AddComponent<AudioSource>();
            source = GetComponents<AudioSource>();
        }

        //> PLAY ONE SHOT SOUND AT POINT IN WORLD
        public void PlayAtPoint(string name, Vector3 point)
        {
            SFX sfx = soundEffects.First(s => s.name == name);
            AudioSource.PlayClipAtPoint(sfx.audio, point, sfx.volume);
            // else Debug.LogWarning($"Unable to find sound: <color=yellow>{name}</color>");
        }

        //> PLAY ONE SHOT SOUND CLIP
        public void PlayOneShot(string name)
        {
            SFX sfx = soundEffects.First(s => s.name == name);
            source[0].PlayOneShot(sfx.audio, sfx.volume);
            // else Debug.LogWarning($"Unable to find sound: <color=yellow>{name}</color>");
        }

        //> REPLACE STREAM SOUND CLIP
        public void Play(string sound, int track = 0)
        {
            SFX sfx = soundEffects.First(s => s.name == sound);
            source[track].clip = sfx.audio;
            source[track].pitch = sfx.pitch;
            source[track].volume = sfx.volume;
            source[track].Play();
        }

        //> STOP STREAM SOUND CLIP
        public void Stop(int stream) => source[stream].Stop();
    }
}