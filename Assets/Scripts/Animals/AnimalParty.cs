using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalParty : MonoBehaviour
{
    [SerializeField] List<Animal> animals;

    public List<Animal> Animals
    {
        get
        {
            return animals;
        }
    }

    private void Start()
    {
        foreach (var animal in animals) //for loop for every animal in animal list
        {
            animal.Init();
        }
    }

    public Animal GetHealthyPokemon()
    {
        //Get first animal in the party that is no fainted
        //If all animal fainted it will return null
        return animals.Where(x => x.HP > 0).FirstOrDefault();
    }

}
