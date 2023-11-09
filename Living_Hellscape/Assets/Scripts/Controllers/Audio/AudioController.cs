using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    public Sound[] sounds;
    
    void Awake ()
    {
        Instance = this;
     
        DontDestroyOnLoad(gameObject);

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
        
    }

    
    public void PlaySoundEffect (string name)
    {
        Sound sound = Array.Find(sounds, sounds => sounds.name == name);
        if (sound != null) 
        {
            sound.source.Play();
        }
        
    }
}