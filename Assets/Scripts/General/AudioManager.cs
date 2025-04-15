using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public AudioMixer audioMixer;
    public float fadeOutDuration = 2f;
    public static AudioManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

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

    public IEnumerator CrossfadeCoroutine(string fromName, string toName)
    {
        Sound from = Array.Find(sounds, s => s.name == fromName);
        Sound to = Array.Find(sounds, s => s.name == toName);

        if (from == null || to == null) yield break;

        float t = 0f;
        float duration = fadeOutDuration;

        float fromStartVolume = from.source.volume;
        float toTargetVolume = to.volume;

        // Start the fade-in sound
        to.source.volume = 0f;
        to.source.Play();

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;

            from.source.volume = Mathf.MoveTowards(from.source.volume, 0f, fromStartVolume * Time.deltaTime / duration);
            to.source.volume = Mathf.MoveTowards(to.source.volume, toTargetVolume, toTargetVolume * Time.deltaTime / duration);

            yield return null;
        }

        // Make sure final values are exact
        from.source.volume = 0f;
        from.source.Stop();

        to.source.volume = toTargetVolume;
    }
}
