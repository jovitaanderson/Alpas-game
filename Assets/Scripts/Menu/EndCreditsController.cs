using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndCreditsController : MonoBehaviour
{
    private string levelToLoad;
    [Header("Audio Settings")]
    [SerializeField] string sceneMusic;


    private void Awake()
    {
        if (sceneMusic != null)
            AudioManager.i.Play(sceneMusic, fade: true); 
    }

    public void Start()
    {
        Debug.Log("Endcreditcontroller start");
        EssentialObjects.i.Destory();
    }

    public void ExitButton()
    {
        AudioManager.i.PlaySfx(AudioId.UISelect);
        //Application.Quit();
        //TODO: quit game
        Application.OpenURL("https://itch.io/");
    }

    public void MainMenuButton()
    {
        AudioManager.i.PlaySfx(AudioId.UISelect);
        SceneManager.LoadScene(0);
    }

    
}
