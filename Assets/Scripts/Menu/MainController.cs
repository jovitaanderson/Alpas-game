using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] AudioClip sceneMusic;
    [SerializeField] string playSceneMusic;

    [Header("Volume Settings")]
    [SerializeField] private Text musicTextValue = null;
    [SerializeField] private Slider musicSlider = null;
    [SerializeField] private Text sfxTextValue = null;
    [SerializeField] private Slider sfxTextSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    [Header("New Game Dialog")]
    [SerializeField] private Text newGameDialogText = null;
    [SerializeField] Color defaultTextColor;
    [SerializeField] Color warningTextColor;
    [SerializeField] string NoExistingSavedGameText;
    [SerializeField] string HaveExistingSavedGameText;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;

    [Header("Game to Load")]
    public string _newGameLevel;
    public string gameToLoad;

    [Header("Create User Name")]
    public InputField displayName;

    [SerializeField] private GameObject noSavedGameDialog = null;
    [SerializeField] private GameObject loadGameDialog = null;

    public static bool checkLoadGame = false;

    public static float musicVolume { get; private set; }

    public static float soundEffectsVolume { get; private set; }

    private void Awake()
    {
        //if (sceneMusic != null)
        //    AudioManager.i.PlayMusic(sceneMusic, fade: true);
        if (playSceneMusic != null)
            AudioManager.i.Play(playSceneMusic);
    }

    private void Start()
    {
        backButton();
    }

    public void backButton()
    {
        if (PlayerPrefs.HasKey("masterVolume") && PlayerPrefs.HasKey("masterSFX"))
        {
            SetMusicVolume(PlayerPrefs.GetFloat("masterVolume"));
            SetSoundEffectsVolume(PlayerPrefs.GetFloat("masterSFX"));
        }
        else
        {
            SetMusicVolume(defaultVolume);
            SetSoundEffectsVolume(defaultVolume);
        }
    }

    public void NewGameDialogText()
    {
        if (PlayerPrefs.HasKey(gameToLoad))
        {
            newGameDialogText.color = warningTextColor;
            newGameDialogText.text = HaveExistingSavedGameText;      
        }
        else
        {
            newGameDialogText.color = defaultTextColor;
            newGameDialogText.text = NoExistingSavedGameText;
        }
    }

    public void NewGameDialogYes()
    {
        PlayerPrefs.SetString("user_name", displayName.text);
        PlayerPrefs.Save();
        PlaySFX();
        SceneManager.LoadScene(_newGameLevel);
    }

    public void LoadGameDialogCheckState()
    {
        if (PlayerPrefs.HasKey(gameToLoad))
            loadGameDialog.SetActive(true);
        else
            noSavedGameDialog.SetActive(true);
    }

    public void LoadGameDialogYes()
    {
        PlaySFX();
        checkLoadGame = true;
        SceneManager.LoadScene(_newGameLevel);
    }


    public void ExitButton()
    {
        PlaySFX();
        //Application.Quit();
        //TODO: quit game
        //Application.OpenURL("https://itch.io/");
        Application.ExternalEval("window.open('" + "https://itch.io/" + "','_self')");
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicTextValue.text = ((int)(volume * 100)).ToString();
        musicSlider.value = volume;
        AudioManager.i.UpdateMixerVolume(volume, soundEffectsVolume);
    }

    public void SetSoundEffectsVolume(float volume)
    {
        soundEffectsVolume = volume;
        sfxTextValue.text = ((int)(volume * 100)).ToString();
        sfxTextSlider.value = volume;
        AudioManager.i.UpdateMixerVolume(musicVolume, volume);

    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", musicVolume);
        PlayerPrefs.SetFloat("masterSFX", soundEffectsVolume);
        PlaySFX();
        PlayerPrefs.Save();
        //Show Prompt
        StartCoroutine(ConfirmationBox());
    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            musicVolume = defaultVolume;
            soundEffectsVolume = defaultVolume;
            musicSlider.value = defaultVolume;
            musicTextValue.text = ((int)(defaultVolume * 100)).ToString();
            sfxTextSlider.value = defaultVolume;
            sfxTextValue.text = ((int)(defaultVolume * 100)).ToString();
            VolumeApply();
        }
    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2f);
        confirmationPrompt.SetActive(false);
    }

    public void PlaySFX()
    {
        AudioManager.i.PlaySfx(AudioId.UISelect);
    }
}
