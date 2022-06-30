using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalCharacterManager : MonoBehaviour
{
    public AnimalCharacterDatabase characterDB;

    public Text nameText;
    public Text type1;
    //public Text type2;
    public Image animalSprite;
    public Text locationText;
    public Text descrtipionText;

    private int selectedOption = 0;

    private void Start()
    {
        
    }

    public void HandleUpdate(Action onBack)
    {
        if (Input.GetKeyDown(KeybindManager.i.keys["BACK"]))
            onBack?.Invoke();

        if (Input.GetKeyDown(KeybindManager.i.keys["RIGHT"]))
            ++selectedOption;
        else if (Input.GetKeyDown(KeybindManager.i.keys["LEFT"]))
            --selectedOption;

        if (selectedOption >= characterDB.CharacterCount)
        {
            selectedOption = 0;
        }
        else if (selectedOption < 0)
        {
            selectedOption = characterDB.CharacterCount - 1;
        }

        UpdateCharacter(selectedOption);
    }

    private void UpdateCharacter(int selectedOption)
    {
        AnimalCharacter character = characterDB.GetCharacter(selectedOption);
        if (character.seen == true)
        {
            animalSprite.sprite = character._base.FrontSprite;
            animalSprite.color = Color.white;
            nameText.text = $"Name: {character._base.Name}";
            type1.text = $"Type: {character._base.Type1}, {character._base.Type2}";
            locationText.text = $"Map locations: {character.locations}";
            descrtipionText.text = character._base.Description;
        }
        else {
            animalSprite.sprite = character._base.FrontSprite;
            animalSprite.color = Color.black;
            nameText.text = $"Name: ????";
            type1.text = $"Type: ????, ????";
            locationText.text = $"Map locations: {character.locations}";
            descrtipionText.text = "????";
        }
    }

}
