using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioData> sfxList;

    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup soundEffectsMixerGroup;

    [SerializeField] private Sound[] sounds;
    
    //[SerializeField] AudioSource musicPlayer;
    //[SerializeField] AudioSource sfxPlayer;

    [SerializeField] float fadeDuration;

    string currMusic;
    float originalMusicVol;
    Dictionary<AudioId, AudioData> sfxLookUp;

    public static AudioManager i { get; private set; }
    private void Awake()
    {
        i = this;

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.AudioClip;
            s.source.volume = s.volume;

            switch (s.audioType)
            {
                case Sound.AudioTypes.soundEffect:
                    s.source.outputAudioMixerGroup = soundEffectsMixerGroup;
                    break;
                case Sound.AudioTypes.music:
                    s.source.outputAudioMixerGroup = musicMixerGroup;
                    break;
            }

        }
    }

    private void Start()
    {
        //if (PlayerPrefs.HasKey("masterVolume"))
        //    musicPlayer.volume = PlayerPrefs.GetFloat("masterVolume");

        //originalMusicVol = musicPlayer.volume;

        sfxLookUp = sfxList.ToDictionary(x => x.id);
    }

    public void PlaySfx(string clipname, bool pauseMusic = false)
    {
        if (clipname == null) return;
        Sound s = Array.Find(sounds, dummySound => dummySound.clipName == clipname);
        if (s == null)
        {
            Debug.LogError("Sound: " + clipname + "does not exist!");
            return;
        }
        //if (pauseMusic)
        //{
        //    s.source.Pause();
        //    StartCoroutine(UnPauseMusic(s.AudioClip.length, s));
        //}

        //playing this clip wont cancel any current music that is played
        s.source.PlayOneShot(s.AudioClip);
    }

    public void PlaySfx(AudioId audioId, bool pauseMusic=false)
    {
        if (!sfxLookUp.ContainsKey(audioId)) return;

        var audioData = sfxLookUp[audioId];
        PlaySfx(audioData.clip, pauseMusic);
    }

   

    public void Play(string clipname, bool loop = true)
    {
        if (clipname == null || clipname == currMusic) return;
        currMusic = clipname;

        Sound s = Array.Find(sounds, dummySound => dummySound.clipName == clipname);
        if (s == null)
        {
            Debug.LogError("Sound: " + clipname + "does not exist!");
            return;
        }
        s.source.loop = loop;
            
        s.source.Play();
    }

    public void UpdateMixerVolume(float musicVolume, float sfxVolume)
    {
        musicMixerGroup.audioMixer.SetFloat("Music Volume", Mathf.Log10(musicVolume) * 20);
        soundEffectsMixerGroup.audioMixer.SetFloat("Sound Effects Volume", Mathf.Log10(sfxVolume) * 20);
    }

    //IEnumerator UnPauseMusic(float delay, Sound s)
    //{
    //    yield return new WaitForSeconds(delay);

    //    s.volume = 0;
    //    s.source.UnPause();
    //    s.DOFade(originalMusicVol, fadeDuration);
    //}

    //public void PlayMusic(AudioClip clip, bool loop = true, bool fade = false)
    //{
    //    //if (clip == null || clip == currMusic) return;

    //    //currMusic = clip;

    //    //StartCoroutine(PlayMusicAsync(clip, loop, fade));
    //}

    //IEnumerator PlayMusicAsync(AudioClip clip, bool loop, bool fade)
    //{
    //    //if (fade)
    //    //    yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();

    //    //musicPlayer.clip = clip;
    //    //musicPlayer.loop = loop;
    //    //musicPlayer.Play();

    //    //if (fade)
    //    //    yield return musicPlayer.DOFade(originalMusicVol, fadeDuration).WaitForCompletion();

    //}

    //public void PlaySfx(AudioClip clip, bool pauseMusic = false)
    //{
    //    //if (clip == null) return;

    //    //if (pauseMusic)
    //    //{
    //    //    musicPlayer.Pause();
    //    //    StartCoroutine(UnPauseMusic(clip.length));
    //    //}

    //    ////playing this clip wont cancel any current music that is played
    //    //sfxPlayer.PlayOneShot(clip);
    //}

}

public enum AudioId { UISelect, Hit, Faint, ExpGain, ItemObtained, AnimalObtained }

[System.Serializable]
public class AudioData
{
    public AudioId id;
    public string clip;
}
