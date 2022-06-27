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
