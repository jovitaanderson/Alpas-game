using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDB
{
    static Dictionary<string, AnimalBase> animals;

    public static void Init()
    {
        animals = new Dictionary<string, AnimalBase>();

        var animalAArray = Resources.LoadAll<AnimalBase>("");
        foreach (var animal in animalAArray)
        {
            if (animals.ContainsKey(animal.Name))
            {
                Debug.LogError($"There are two animals with the name {animal.Name}");
                continue;
            }

            animals[animal.Name] = animal;
        }
    }

    public static AnimalBase GetAnimalByName(string name)
    {
        if (!animals.ContainsKey(name))
        {
            Debug.LogError($"Animal with name {name} not found in the datebase");
            return null;
        }

        return animals[name];
    }
}
