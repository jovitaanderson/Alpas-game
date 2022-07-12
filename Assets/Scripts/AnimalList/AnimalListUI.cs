using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalListUI : MonoBehaviour
{
    //AnimalCharacterDatabase characterDB;
    List<AnimalCharacter> animalsSeenData;

    public Text nameText;
    public Text type1;
    //public Text type2;
    public Image animalSprite;
    public Text locationText;
    public Text descrtipionText;

    private int selectedOption = 0;

    AnimalList animalList;

    private void Start()
    {
        animalList = this.GetComponentInParent<AnimalList>();
        animalsSeenData = this.GetComponentInParent<AnimalList>().animalsSeenData;
        animalList.OnUpdated += UpdateAnimalSeenList;
    }

    public void UpdateAnimalSeenList()
    {
        animalsSeenData = this.GetComponentInParent<AnimalList>().animalsSeenData;
    }

    public void HandleUpdate(Action onBack)
    {
        if (Input.GetKeyDown(SettingsManager.i.getKey("BACK")) || Input.GetKeyDown(SettingsManager.i.getKey("BACK1")))
            onBack?.Invoke();

        if (Input.GetKeyDown(SettingsManager.i.getKey("RIGHT")) || Input.GetKeyDown(SettingsManager.i.getKey("RIGHT1")))
            ++selectedOption;
        else if (Input.GetKeyDown(SettingsManager.i.getKey("LEFT")) || Input.GetKeyDown(SettingsManager.i.getKey("LEFT1")))
            --selectedOption;

        if (selectedOption >= animalsSeenData.Count)
        {
            selectedOption = 0;
        }
        else if (selectedOption < 0)
        {
            selectedOption = animalsSeenData.Count - 1;
        }

        UpdateCharacter(selectedOption);
    }

    private void UpdateCharacter(int selectedOption)
    {
        AnimalCharacter character = animalsSeenData[selectedOption];
        AnimalBase _base = character._base;
        if (character.seen == true)
        {
            
            animalSprite.sprite = _base.FrontSprite;
            animalSprite.color = Color.white;
            nameText.text = $"Name: {_base.Name}";
            type1.text = $"Type: {_base.Type1}, {_base.Type2}";
            locationText.text = $"Map locations: {character.locations}";
            descrtipionText.text = _base.Description;
        }
        else {
            animalSprite.sprite = _base.FrontSprite;
            animalSprite.color = Color.black;
            nameText.text = $"Name: ????";
            type1.text = $"Type: ????, ????";
            locationText.text = $"Map locations: {character.locations}";
            descrtipionText.text = "????";
        }
    }
}


