using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    public Sound[] sounds;

    [SerializeField]
    bool logError;

    public Sound currentMusic;

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
            return;
        }

        LogError(name);
    }

    public void StopSoundEffect(string name)
    {
        Sound sound = Array.Find(sounds, sounds => sounds.name == name);
        if (sound != null)
        {
            sound.source.Stop();
        }

        LogError(name);
    }

    public IEnumerator SetMusic(string name, float duration)
    {
        if (name == "nomusic")
        {
            yield return StartCoroutine(FadeOutSoundEffect(currentMusic.name, duration));
            currentMusic = null;
            yield break;
        }

        Sound sound = Array.Find(sounds, sounds => sounds.name == name);
        //Debug.Log("Hello there!");

        if (sound == null)
        {
            LogError(name);
            yield break;
        }

        if (currentMusic != null)
        {
            yield return StartCoroutine(FadeOutSoundEffect(currentMusic.name, duration / 2));
            yield return StartCoroutine(FadeInSoundEffect(name, duration / 2));
        }
        else
        {
            yield return StartCoroutine(FadeInSoundEffect(name, duration));
        }
        currentMusic = sound;
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
        else
        {
            LogError(name);
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
        else
        {
            LogError(name);
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
        if (playWalk && !GameController.Instance.StopUpdates)
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

    void LogError(string name)
    {
        if (logError)
        {
            Debug.LogWarning("SOUND: " + name + " NOT FOUND");
        }
    }
}

