using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalParty : MonoBehaviour
{
    [SerializeField] List<Animal> animals;

    public event Action OnUpdated;

    public AnimalStorage animalStorage;

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
        animalStorage = GetComponent<AnimalStorage>();
    }

    public Animal GetHealthyAnimal()
    {
        //Get first animal in the party that is no fainted
        //If all animal fainted it will return null
        return animals.Where(x => x.HP > 0).FirstOrDefault();
    }

    public void resetStatsAnimal()
    {
        foreach (var animal in animals) //for loop for every animal in animal list
        {
            //set animal back to full HP
            animal.Heal();
            //cure all status of animal
            animal.CureStatus();

            //playerParty.Animals.ForEach(p => p.Heal());
            //animal.Init();
        }
    }

    //add animals into the party only if the party has less than 6 animals
    public void AddAnimal(Animal newAnimal) 
    {
        if (animals.Count < 6)
        {
            animals.Add(newAnimal);
            OnUpdated?.Invoke();

        } else {
            animalStorage.AddAnimal(newAnimal);
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
            //Debug.Log(evolution.evolutionMessageState);
            if (evolution != null)
            {
                if (evolution.evolutionMessageState == false)
                {
                    //dont evolve immediately, give a dialog to say that animal is ready for evolution
                    yield return DialogManager.Instance.ShowDialogText($"{animal.Base.Name} is ready for evolution");
                    evolution.evolutionMessageState = true;
                    //yield return EvolutionManager.i.Evolve(animal, evolution);
                }
            }

        }

    }

    public void PartyUpdated()
    {
        OnUpdated?.Invoke();
    }

    public static AnimalParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<AnimalParty>();
    }

}
