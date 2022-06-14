using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//To control the wild pokemons spwaning in a region (map)
public class MapArea : MonoBehaviour
{
    [SerializeField] List<AnimalEncounterRecord> wildAnimals;

    private void Start()
    {
        int totalChance = 0;
        foreach(var record in wildAnimals)
        {
            record.chanceLower = totalChance;
            record.chanceUpper = totalChance + record.chancePercentage;

            totalChance = totalChance + record.chancePercentage;
        }
    }

    public Animal GetRandomWildAnimal()
    {
        int randVal = Random.Range(1, 101);
        var animalRecord = wildAnimals.First(p => randVal >= p.chanceLower && randVal <= p.chanceUpper);

        var levelRange = animalRecord.levelRange;
        int level = levelRange.y == 0 ? levelRange.x : Random.Range(levelRange.x, levelRange.y + 1);

        var wildAnimal = new Animal(animalRecord.animal, level);
        wildAnimal.Init();
        return wildAnimal;

    }
}

[System.Serializable]
public class AnimalEncounterRecord
{
    public AnimalBase animal;
    public Vector2Int levelRange;
    public int chancePercentage;

    public int chanceLower { get; set; }
    public int chanceUpper { get; set; }
}
