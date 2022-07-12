using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalList : MonoBehaviour, ISavable
{
    [SerializeField] public GameObject animalListUI;
    [SerializeField] public List<AnimalCharacter> animalsSeenData;

    public event Action OnUpdated;

    public void Start()
    {
        //todo: REMOVE once we done with animal descriptions
        SetAllTrue();
    }

    public void AnimalSeen(Animal animal)
    {
        foreach (var animalChar in animalsSeenData)
        {
            if (animalChar._base.Name == animal.Base.Name)
            {
                animalChar.seen = true;
                break;
            }
        }
        OnUpdated?.Invoke();
    }

    public void SetAllTrue()
    {
        foreach (var animalChar in animalsSeenData)
        {
            animalChar.seen = true;
        }
        OnUpdated?.Invoke();
    }

    public void HandleUpdate(Action onBack)
    {
        animalListUI.GetComponent<AnimalListUI>().HandleUpdate(onBack);
    }

    public object CaptureState()
    {
        var saveData = new AnimalSeenSaveData()
        {
            animalsSeenData = animalsSeenData.Select(p => p.GetSaveData()).ToList()
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (AnimalSeenSaveData)state;
        animalsSeenData = saveData.animalsSeenData.Select(s => new AnimalCharacter(s)).ToList();

        OnUpdated?.Invoke();
    }
}

[Serializable]
public class AnimalSeenSaveData
{
    public List<AnimalCharacterSaveData> animalsSeenData;
}

[System.Serializable]
public class AnimalCharacter
{
    public AnimalBase _base;
    public string locations;
    public bool seen;

    public AnimalCharacter(AnimalCharacterSaveData saveData)
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
    }
}

[System.Serializable]
public class AnimalCharacterSaveData
{
    public string name;
    public string locations;
    public bool seen;
}
