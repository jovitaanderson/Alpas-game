using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalParty : MonoBehaviour
{
    [SerializeField] List<Animal> animals;

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

    public Animal GetHealthyAnimal()
    {
        //Get first animal in the party that is no fainted
        //If all animal fainted it will return null
        return animals.Where(x => x.HP > 0).FirstOrDefault();
    }

    //add animals into the party only if the party has less than 6 animals
    public void AddAnimal(Animal newAnimal) 
    {
        if (animals.Count < 6)
        {
            animals.Add(newAnimal);
            OnUpdated?.Invoke();
        } else {

            //TODO: Add to the PC once thats implenmented
        }
    }

    public IEnumerator CheckForEvolutions()
    {
        foreach (var animal in animals)
        {
            var evolution = animal.CheckForEvolution();
            //return true if player has 1 coin
            bool hasItem = Inventory.GetInventory().HasItem(evolution.RequiredItem);
            if (evolution != null) //animal has required lvl to evolve 
            {
                if (hasItem)
                {  //and has 1 coin
                    //Use one coin to evovle
                    Inventory.GetInventory().RemoveItem(evolution.RequiredItem);



                    //run question
                    yield return EvolutionManager.i.Evolve(animal, evolution);
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialogText($"You do not have enough coins to evolve your animal!");
                }
            }
        }

    }

    public static AnimalParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<AnimalParty>();
    }

}
