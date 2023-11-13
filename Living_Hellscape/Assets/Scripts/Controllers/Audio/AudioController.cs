using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    public Sound[] sounds;

    void Awake()
    {
        Instance = this;

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.playOnAwake = false;
        }

    }


    public void PlaySoundEffect(string name)
    {
        Sound sound = Array.Find(sounds, sounds => sounds.name == name);
        if (sound != null)
        {
            sound.source.Play();
        }
    }

    public void StopSoundEffect(string name)
    {
        Sound sound = Array.Find(sounds, sounds => sounds.name == name);
        if (sound != null)
        {
            sound.source.Stop();
        }
    }

    public IEnumerator FadeInSoundEffect(string name, float duration)
    {
        Sound sound = Array.Find(sounds, sounds => sounds.name == name);
        if (sound != null)
        {
            sound.source.Play();
            float currentDuration = 0;

            while (currentDuration < duration)
            {
                currentDuration += Time.deltaTime;
                float t = Mathf.InverseLerp(0, duration, currentDuration);
                sound.source.volume = t * sound.volume;

                yield return null;
            }
            sound.source.volume = 1 * sound.volume;
        }
    }

    public IEnumerator FadeOutSoundEffect(string name, float duration)
    {
        Sound sound = Array.Find(sounds, sounds => sounds.name == name);
        if (sound != null)
        {
            float currentDuration = duration;

            while (currentDuration > 0)
            {
                currentDuration -= Time.deltaTime;
                float t = Mathf.InverseLerp(0, duration, currentDuration);
                sound.source.volume = t * sound.volume;

                yield return null;
            }
            sound.source.Stop();
            sound.source.volume = 0 * sound.volume;
        }
    }


    private bool playWalk = false;
    private bool alternateWalkSound = false;
    private float walkTimer = 0.25f;
    private float currentWalkTimer = 0;

    public void PlayWalkSound()
    {
        if (!playWalk)
        {
            playWalk = true;
        }
    }

    public void StopWalkSound()
    {
        if (playWalk)
        {
            playWalk = false;
            currentWalkTimer = 0;
            alternateWalkSound = false;
        }
    }

    private void Update()
    {
        if (playWalk)
        {
            if (currentWalkTimer <= 0)
            {
                if (!alternateWalkSound)
                {
                    PlaySoundEffect("testwalk1");
                    alternateWalkSound = true;
                }
                else
                {
                    PlaySoundEffect("testwalk2");
                    alternateWalkSound = false;
                }
                currentWalkTimer = walkTimer;
            }
            else
            {
                currentWalkTimer -= Time.deltaTime;
            }
        }
    }
}

