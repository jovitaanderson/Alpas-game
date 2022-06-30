using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalStorage : MonoBehaviour
{
    [SerializeField] List<Animal> animals;

    public int maxAnimalsInStorage = 20;
    public event Action OnUpdated;

    public List<Animal> Animals
    {
        get
        {
            return animals;
        }
        set
        {
            animals = value;
            OnUpdated?.Invoke();
        }
    }

    private void Awake()
    {
        foreach (var animal in animals) //for loop for every animal in animal list
        {
            animal.Init();
        }
    }

    private void Start()
    {

    }

    //add animals into the party only if the party has less than 6 animals
    public void AddAnimal(Animal newAnimal)
    {
        if (animals.Count < maxAnimalsInStorage)
        {
            animals.Add(newAnimal);
            OnUpdated?.Invoke();
        }
        else
        {
            StartCoroutine("Release some animals to capture more animals.");
        }
    }

    public bool CheckForEvolutions()
    {
        return animals.Any(p => p.CheckForEvolution() != null);
    }

    public IEnumerator RunEvolutions()
    {
        foreach (var animal in animals)
        {

            var evolution = animal.CheckForEvolution();
            if (evolution != null)
            {
                if (evolution.evolutionMessageState == false)
                {
                    yield return DialogManager.Instance.ShowDialogText($"{animal.Base.Name} is ready for evolution");
                    evolution.evolutionMessageState = true;
                }
            }
        }
    }

    public void StorageUpdated()
    {
        OnUpdated?.Invoke();
    }

    public static AnimalStorage GetPlayerStorage()
    {
        return FindObjectOfType<PlayerController>().GetComponent<AnimalStorage>();
    }

}

