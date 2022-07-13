using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LoadPrefs : MonoBehaviour
{
    [Header("General Setting")]
    [SerializeField] private MainController mainController;

    private void Awake()
    {
       
        if (PlayerPrefs.HasKey("masterVolume") && PlayerPrefs.HasKey("masterSFX"))
        {
            mainController.SetMusicVolume(PlayerPrefs.GetFloat("masterVolume"));
            mainController.SetSoundEffectsVolume(PlayerPrefs.GetFloat("masterSFX"));
        }
        else
        {
            mainController.ResetButton("Audio");
        }

    }
}
