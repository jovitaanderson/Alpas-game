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
    public Text type2;
    public Image animalSprite;
    public Text locationText;

    private int selectedOption = 0;

    private void Start()
    {
        
    }

    public void HandleUpdate(Action onBack)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            onBack?.Invoke();

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selectedOption;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
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
        animalSprite.sprite = character.animalSprite;
        nameText.text = character.animalName;
        type1.text = character.type1.ToString();
        type2.text = character.type2.ToString();
        locationText.text = character.locations;
    }
}
