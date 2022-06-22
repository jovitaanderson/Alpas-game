using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] AudioClip sceneMusic;

    [Header("Volume Settings")]
    [SerializeField] private Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
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

    [SerializeField] private GameObject noSavedGameDialog = null;
    [SerializeField] private GameObject loadGameDialog = null;

    public static bool checkLoadGame = false;

    private void Awake()
    {
        if (sceneMusic != null)
            AudioManager.i.PlayMusic(sceneMusic, fade: true);
    }

    private void Start()
    {
        //TODO: remove if audio is still saved (when restarting game) without this code
        if (PlayerPrefs.HasKey("masterVolume"))
            SetVolume(PlayerPrefs.GetFloat("masterVolume"));
        else
            SetVolume(defaultVolume);

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
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
        volumeSlider.value = volume;
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        PlaySFX();
        //Show Prompt
        StartCoroutine(ConfirmationBox());
    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            PlaySFX();
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
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
