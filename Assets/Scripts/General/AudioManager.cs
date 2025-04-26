using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public AudioMixer audioMixer;
    public float fadeOutDuration = 2f;
    public static AudioManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            s.source.outputAudioMixerGroup = s.outputGroup;
        }
    }

    public void Play(string name, bool? loop = null)
    {
        Sound s = Array.Find(sounds, s => s.name == name);
        if (s == null) return;
        s.source.loop = loop ?? s.loop;
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, s => s.name == name);
        if (s == null || !s.source.isPlaying) return;
        s.source.Stop();     
    }

    public void SetMusicVolume(float volume)
    {
        if (volume <= 0f)
            audioMixer.SetFloat("MusicVolume", -80f);
        else
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetSEVolume(float volume)
    {
        if (volume <= 0f)
            audioMixer.SetFloat("SEVolume", -80f);
        else
            audioMixer.SetFloat("SEVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetMasterVolume(float MusicVolume, float SFXVolume)
    {
        SetMusicVolume(MusicVolume);
        SetSEVolume(SFXVolume);
    }
    public IEnumerator FadeOutCoroutine(string name)
    {
        Sound s = Array.Find(sounds, s => s.name == name);
        if (s == null) yield break;
        float startVolume = s.source.volume;

        while (s.source.volume > 0)
        {
            s.source.volume = Mathf.MoveTowards(s.source.volume, 0f, startVolume / fadeOutDuration * Time.deltaTime);
            yield return null;
        }

        s.source.Stop();
        s.source.volume = startVolume;
    }

    public IEnumerator CrossfadeCoroutine(string toName)
    {
        Sound from = sounds.FirstOrDefault(s =>
       s.source != null &&
       s.source.isPlaying &&
       s.source.outputAudioMixerGroup != null &&
       s.source.outputAudioMixerGroup.name == "Music");

        Sound to = Array.Find(sounds, s => s.name == toName);
        if (from == null || to == null) yield break;

        // Fade out the current music (Ensure it's completely stopped)
        yield return StartCoroutine(FadeOutCoroutine(from.name));
        // Prepare and play the new sound
        to.source.volume = 0f; // Start the new music muted
        to.source.Play();

        // Fade in the new music
        float t = 0f;
        float duration = fadeOutDuration;
        float fromStartVolume = from.source.volume;
        float toTargetVolume = to.volume;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;

            from.source.volume = Mathf.Lerp(fromStartVolume, 0f, normalized);
            to.source.volume = Mathf.Lerp(0f, toTargetVolume, normalized);

            yield return null;
        }

        // Ensure the volumes are exactly what we want
        from.source.volume = 0f;
        from.source.Stop();

        to.source.volume = toTargetVolume;

        Debug.Log("Crossfade complete from " + from.name + " to " + to.name);
    }

}
