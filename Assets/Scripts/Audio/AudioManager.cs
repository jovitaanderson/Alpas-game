using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AllMusic sounds;
    [SerializeField] List<AudioData> sfxList;

    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup soundEffectsMixerGroup;

    
    
    //[SerializeField] AudioSource musicPlayer;
    //[SerializeField] AudioSource sfxPlayer;

    [SerializeField] float fadeDuration;

    string currMusic;
    float originalMusicVol;
    float originalSFXVol;
    string prevMusic;
    Dictionary<AudioId, AudioData> sfxLookUp;

    public static AudioManager i { get; private set; }
    private void Awake()
    {
        i = this;

        for(int s = 0; s < sounds.SoundCount; s++)
        {
            Sound sound = sounds.GetSoundIndex(s);
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.AudioClip;
            sound.source.volume = sound.volume;

            switch (sound.audioType)
            {
                case Sound.AudioTypes.soundEffect:
                    sound.source.outputAudioMixerGroup = soundEffectsMixerGroup;
                    break;
                case Sound.AudioTypes.music:
                    sound.source.outputAudioMixerGroup = musicMixerGroup;
                    break;
            }

        }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("masterVolume") && PlayerPrefs.HasKey("masterSFX"))
        {
            originalMusicVol = PlayerPrefs.GetFloat("masterVolume");
            originalSFXVol = PlayerPrefs.GetFloat("masterSFX");
        }
        sfxLookUp = sfxList.ToDictionary(x => x.id);
    }

    public void PlaySfx(string clipname, bool pauseMusic = false)
    {
        if (clipname == null) return;
        Sound s = sounds.GetSound(clipname);
        //Sound s = Array.Find(sounds, dummySound => dummySound.clipName == clipname);
        if (s == null)
        {
            Debug.LogError("Sound: " + clipname + "does not exist!");
            return;
        }
        if (pauseMusic && prevMusic != null)
        {
            Sound prevSound = sounds.GetSound(prevMusic);
            prevSound.source.Pause();
            StartCoroutine(UnPauseMusic(s.AudioClip.length, prevSound));
        }

        //playing this clip wont cancel any current music that is played
        s.source.PlayOneShot(s.AudioClip);
    }

    public void PlaySfx(AudioId audioId, bool pauseMusic=false)
    {
        if (!sfxLookUp.ContainsKey(audioId)) return;

        var audioData = sfxLookUp[audioId];
        PlaySfx(audioData.clip, pauseMusic);
    }

    

    public void Play(string clipname, bool loop = true, bool fade = false)
    {
        if (clipname == null || clipname == currMusic) return;
        currMusic = clipname;

        StartCoroutine(PlayMusicAsync(clipname, loop, fade));
    }

    IEnumerator PlayMusicAsync(string clipname, bool loop, bool fade)
    {
        prevMusic = currMusic;
        Sound s = sounds.GetSound(clipname);
        //Sound s = Array.Find(sounds, dummySound => dummySound.clipName == clipname);
        if (s == null)
        {
            Debug.LogError("Sound: " + clipname + "does not exist!");
            yield break;
        }
        if (fade)
            yield return s.source.DOFade(0, fadeDuration).WaitForCompletion();

        currMusic = clipname;
        s.source.loop = loop;
        s.source.Play();

        if (fade)
            yield return s.source.DOFade(originalMusicVol, fadeDuration).WaitForCompletion();
    }


    public void Stop()
    {
        for (int s = 0; s < sounds.SoundCount; s++)
        {
            Sound sound = sounds.GetSoundIndex(s);
            if (sound.source.isPlaying)
            {
                sound.source.Stop();
            }
        }
    }

    public void UpdateMixerVolume(float musicVolume, float sfxVolume)
    {
        musicMixerGroup.audioMixer.SetFloat("Music Volume", Mathf.Log10(musicVolume) * 20);
        soundEffectsMixerGroup.audioMixer.SetFloat("Sound Effects Volume", Mathf.Log10(sfxVolume) * 20);
    }

    IEnumerator UnPauseMusic(float delay, Sound s)
    {
        yield return new WaitForSeconds(delay);

        s.source.volume = 0;
        s.source.UnPause();
        s.source.DOFade(originalMusicVol, fadeDuration);
    }


}

public enum AudioId { UISelect, Hit, Faint, ExpGain, ItemObtained, AnimalObtained, OpenChest }

[System.Serializable]
public class AudioData
{
    public AudioId id;
    public string clip;
}
