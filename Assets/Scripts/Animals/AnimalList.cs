using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalList : MonoBehaviour, ISavable
{
    [SerializeField] public GameObject animalListUI;
    [SerializeField] public List<AnimalCharacter> animalsSeenData;
    //public AnimalCharacterDatabase characterDB;

    public void AnimalSeen(Animal animal)
    {
        foreach (var animalChar in animalsSeenData)
        {
            if (animalChar.name == animal.Base.Name)
            {
                animalChar.seen = true;
                break;
            }
        }
    }

    public void HandleUpdate(Action onBack)
    {
        animalListUI.GetComponent<AnimalListUI>().HandleUpdate(onBack);
    }

    public object CaptureState()
    {
        var saveData = new AnimalSeenSaveData()
        {
            //animalsSeenData = this.animalsSeenData
            //animalsSeenData = GetComponent<AnimalCharacterDatabase>().AnimalsSeenData
            animalsSeenData = this.animalsSeenData
        };
        return saveData;
    }

    //Used to restore data when game loaded
    public void RestoreState(object state)
    {
        var saveData = (AnimalSeenSaveData)state;
        //characterDB = ScriptableObject.CreateInstance<AnimalCharacterDatabase>().UpdateData(saveData.characterDB);
        //characterDB = new AnimalCharacterDatabase(saveData.characterDB);
        //GetComponent<AnimalCharacterDatabase>().AnimalsSeenData = saveData.animalsSeenData; 
        //animalsSeenData = GetComponent<AnimalCharacterDatabase>().AnimalsSeenData.Select(p => p.GetSaveData()).ToList();
        //animalCharacters = GetComponent<AnimalParty>().Animals.Select(p => p.GetSaveData()).ToList()
        //characterDB = ScriptableObject.CreateInstance<AnimalCharacterDatabase>();
        animalsSeenData = saveData.animalsSeenData;
    }
}

[System.Serializable]
public class AnimalSeenSaveData
{
    public List<AnimalCharacter> animalsSeenData;
}

[System.Serializable]
public class AnimalCharacter
{
    public string name;
    //public AnimalBase _base;

    public string locations;
    public bool seen;

    /*public AnimalCharacter(AnimalCharacterSaveData saveData)
    {
        _base = AnimalDB.GetObjectByName(saveData.name);
        locations = saveData.locations;
        seen = saveData.seen;
    }

    public AnimalCharacterSaveData GetSaveData()
    {
        var saveData = new AnimalCharacterSaveData()
        {
            name = _base.name,
            locations = locations,
            seen = seen
        };
        return saveData;
    }*/

}

[System.Serializable]
public class AnimalCharacterSaveData
{
    public string name;

    public string locations;
    public bool seen;
}
