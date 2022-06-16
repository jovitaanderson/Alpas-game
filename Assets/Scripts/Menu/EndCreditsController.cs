using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndCreditsController : MonoBehaviour
{
    private string levelToLoad;
    [Header("Audio Settings")]
    [SerializeField] AudioClip sceneMusic;

    private void Awake()
    {
        if (sceneMusic != null)
            AudioManager.i.PlayMusic(sceneMusic, fade: true);
    }

    public void ExitButton()
    {
        AudioManager.i.PlaySfx(AudioId.UISelect);
        Application.Quit();
    }

    public void MainMenuButton()
    {
        AudioManager.i.PlaySfx(AudioId.UISelect);
        SceneManager.LoadScene(0);
    }
}
