using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour {

    [System.Serializable]
    private class Sound
    {
        public string name;

        public AudioClip clip;

        public AudioMixerGroup audioMixer;

        [Range(0f, 2f)]
        public float volume;

        [Range(0.5f, 2f)]
        public float pitch;

        [Range(0f, 1f)]
        public float spatialBlend;

        public bool loop;

        [HideInInspector]
        public AudioSource source;
    }

    [SerializeField] private Sound[] sounds;


    private void Awake()
    {

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.name = s.name;
            s.source.playOnAwake = false;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = s.spatialBlend;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
        this.gameObject.name = "AudioManager";
    }
    public void Play(string name, bool pause, bool playing)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (playing == true)
        {
            if (pause == true)
            {
                if (s != null && s.source)
                {
                    s.source.Pause();
                }
                else
                {
                    Debug.Log(gameObject.name +":"+ name+ " Pause didn't work...");
                }
                pause = false;
            }
            else
            {
                if (s != null && s.source)
                {
                    s.source.UnPause();
                }
                else
                {
                    Debug.Log(gameObject.name + ":" + name + " Unpause didn't work...");
                }
                pause = true;
            }
        }
        else
        {
            if (s != null && s.source)
            {
                s.source.Play();
            }
            else
            {
                Debug.Log(gameObject.name + ":" + name + " Play didn't work...");
            }
        }
    }
}
