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
    
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource sfxPlayer;

    [SerializeField] float fadeDuration;

    string currMusic;
    float originalMusicVol;
    float originalSFXVol;
    Dictionary<AudioId, AudioData> sfxLookUp;

    public static AudioManager i { get; private set; }
    private void Awake()
    {
        i = this;

        for(int s = 0; s < sounds.SoundCount; s++)
        {
            Sound sound = sounds.GetSoundIndex(s);

            switch (sound.audioType)
            {
                case Sound.AudioTypes.soundEffect:
                    sfxPlayer.volume = sound.volume;
                    sfxPlayer.outputAudioMixerGroup = soundEffectsMixerGroup;
                    break;
                case Sound.AudioTypes.music:
                    musicPlayer.volume = sound.volume;
                    musicPlayer.outputAudioMixerGroup = musicMixerGroup;
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
        if (pauseMusic)
        {
            musicPlayer.Pause();
            StartCoroutine(UnPauseMusic(s.AudioClip.length));
        }

        //playing this clip wont cancel any current music that is played
        sfxPlayer.clip = s.AudioClip;
        sfxPlayer.PlayOneShot(s.AudioClip);
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
        Sound s = sounds.GetSound(clipname);
        
        if (s == null)
        {
            Debug.LogError("Sound: " + clipname + "does not exist!");
            yield break;
        }

        if (fade)
            yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();

        currMusic = clipname;
        musicPlayer.clip = s.AudioClip;
        musicPlayer.loop = loop;
        musicPlayer.Play();

        if (fade)
            yield return musicPlayer.DOFade(originalMusicVol, fadeDuration).WaitForCompletion();
    }

    public void UpdateMixerVolume(float musicVolume, float sfxVolume)
    {
        musicMixerGroup.audioMixer.SetFloat("Music Volume", Mathf.Log10(musicVolume) * 20);
        soundEffectsMixerGroup.audioMixer.SetFloat("Sound Effects Volume", Mathf.Log10(sfxVolume) * 20);
    }

    IEnumerator UnPauseMusic(float delay)
    {
        yield return new WaitForSeconds(delay);

        musicPlayer.volume = 0;
        musicPlayer.UnPause();
        musicPlayer.DOFade(originalMusicVol, fadeDuration);
    }


}

public enum AudioId { UISelect, Hit, Faint, ExpGain, ItemObtained, AnimalObtained, OpenChest }

[System.Serializable]
public class AudioData
{
    public AudioId id;
    public string clip;
}
