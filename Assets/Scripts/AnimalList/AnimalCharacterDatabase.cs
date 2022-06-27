using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AnimalCharacterDatabase : ScriptableObject
{
    public AnimalCharacter[] animalCharacter;

    public int CharacterCount
    {
        get
        {
            return animalCharacter.Length;
        }
    }

    public AnimalCharacter GetCharacter(int index)
    {
        return animalCharacter[index];
    }
}

[System.Serializable]
public class AnimalCharacter
{
    public string name;
    public AnimalBase _base;

    public string locations;

    /*public string animalName;
    public AnimalType type1;
    public AnimalType type2;
    public Sprite animalSprite;*/

}



