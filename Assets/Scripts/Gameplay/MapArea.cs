using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//To control the wild pokemons spwaning in a region (map)
public class MapArea : MonoBehaviour
{
    [SerializeField] List<Animal> wildAnimals;
    public Animal GetRandomWildAnimal()
    {
        var wildAnimal =  wildAnimals[Random.Range(0, wildAnimals.Count)];
        wildAnimal.Init();
        return wildAnimal;
    }
}
