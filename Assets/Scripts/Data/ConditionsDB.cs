using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Conditions Database, it will store the list of coniditions inside a dictionary
public class ConditionsDB 
{
    public static void Init()
    {
        //kvp = key value pair
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            //TODO: posion message doesnt apppear after animal faint and come back with new pokemon
            ConditionID.psn, 
            new Condition()
            {
                Name = "Posion",
                StartMessage = "has been poisoned",
                //lamda functions
                OnAfterTurn = (Animal animal) =>
                {
                    animal.DecreaseHP(animal.MaxHp / 8);
                    animal.StatusChanges.Enqueue($"{animal.Base.Name} hurt itself due to posion");
                }

            }
        
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "has been burn",
                //lamda functions
                OnAfterTurn = (Animal animal) =>
                {
                    animal.DecreaseHP(animal.MaxHp / 16);
                    animal.StatusChanges.Enqueue($"{animal.Base.Name} hurt itself due to burn");
                }

            }

        },
         {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralyzed",
                StartMessage = "has been paralyzed",
                //lamda functions, since we use Func it need to return a value (bool)
                //true = pokemon can perform move, false = pokemon cannot perfrom move 
                OnBeforeMove = (Animal animal) =>
                {
                    if( Random.Range(1,5) == 1) {
                        animal.StatusChanges.Enqueue($"{animal.Base.Name}'s paralyzed and can't move");
                        return false; //wont be able to perfrom move
                    }
                    return true; //can perform a move 
                }

            }

         },
         {
            ConditionID.frz,
            new Condition()
            {
                Name = "Freeze",
                StartMessage = "has been frozen",
                OnBeforeMove = (Animal animal) =>
                {
                    if( Random.Range(1,5) == 1) { //theres a 1/4 chance status is cured during a turn
                        animal.CureStatus();
                        animal.StatusChanges.Enqueue($"{animal.Base.Name}'s is not frozen anymore");
                        return true; //can perfrom move
                    }
                    return false; //cannot perform a move 
                }

            }

         },
         {
            //TODO: my pokemon doesnt sleep?
            ConditionID.slp,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = "has fallen asleep",
                OnStart = (Animal animal) =>
                {
                    //Sleep for  a random no. of turns between 1-3
                    animal.StatusTime = Random.Range(1,4);
                    Debug.Log($"Will be asleep for {animal.StatusTime} moves");
                },
                OnBeforeMove = (Animal animal) =>
                {
                   if(animal.StatusTime <= 0)
                   {
                       animal.CureStatus();
                       animal.StatusChanges.Enqueue($"{animal.Base.Name} woke up!");
                       return true;
                   }

                    animal.StatusTime--;
                    animal.StatusChanges.Enqueue($"{animal.Base.Name} is sleeping");
                    return false;
                }
            }

         },
         {
            //Volatile Status Conditions
            ConditionID.confusion,
            new Condition()
            {
                Name = "Confusion",
                StartMessage = "has fallen confused",
                OnStart = (Animal animal) =>
                {
                    //Confused for 1 - 4 turns
                    animal.VolatileStatusTime = Random.Range(1,5);
                    Debug.Log($"Will be confused for {animal.VolatileStatusTime} moves");
                },
                OnBeforeMove = (Animal animal) =>
                {
                   if(animal.VolatileStatusTime <= 0)
                   {
                       animal.CureVolatileStatus();
                       animal.StatusChanges.Enqueue($"{animal.Base.Name} kicked out of confusion!");
                       return true;
                   }

                    animal.VolatileStatusTime--;

                    //50% chance to do a move
                    if(Random.Range(1,3) == 1)
                        return true;

                    //Hurt by confusion
                    animal.StatusChanges.Enqueue($"{animal.Base.Name} is confused");
                    animal.DecreaseHP(animal.MaxHp / 8);
                    animal.StatusChanges.Enqueue($"{animal.Base.Name} hurt itself due to confusion");
                    return false;
                }
            }

         },
         {
            ConditionID.sng,
            new Condition()
            {
                Name = "Sing",
                StartMessage = "is singing",
                //lamda functions
                 OnStart = (Animal animal) =>
                {
                    //Sleep for  a random no. of turns between 1-4
                    animal.StatusTime = Random.Range(1,5);
                    Debug.Log($"Will be self-healing for {animal.StatusTime} moves");
                },
                OnAfterTurn = (Animal animal) =>
                {
                    if(animal.StatusTime <= 0)
                   {
                       animal.CureStatus();
                       animal.StatusChanges.Enqueue($"{animal.Base.Name} has finished singing!");
                   }

                    animal.StatusTime--;
                    animal.IncreaseHP(animal.MaxHp / 8);
                    animal.StatusChanges.Enqueue($"{animal.Base.Name} heal itself due to the melodious singing");
                }

            }

         },
         {
            ConditionID.bld,
            new Condition()
            {
                Name = "Blood Bath",
                StartMessage = "is blood bathing",
                //lamda functions
                OnAfterTurn = (Animal animal) =>
                {
                    animal.DecreaseHP(animal.MaxHp / 16);
                    animal.StatusChanges.Enqueue($"{animal.Base.Name} hurt itself due to the blood bath");
                }

            }

         },
         {
            ConditionID.slr,
            new Condition()
            {
                Name = "Solar Beam",
                StartMessage = "is using solar beaming",
                //lamda functions
               OnStart = (Animal animal) =>
                {
                    //Sleep for  a random no. of turns between 1-4
                    animal.StatusTime = Random.Range(1,5);
                    Debug.Log($"Will be solar beamed for {animal.StatusTime} moves");
                },
                OnAfterTurn = (Animal animal) =>
                {
                   if(animal.StatusTime <= 0)
                   {
                       animal.CureStatus();
                       animal.StatusChanges.Enqueue($"{animal.Base.Name} has recovered from solar beam!");
                       
                   }

                    animal.StatusTime--;
                    animal.DecreaseHP(animal.MaxHp / 12);
                    animal.StatusChanges.Enqueue($"{animal.Base.Name} hurt its eyes due to solar beam");
                    
                }

            }

         },
         {
            ConditionID.bbl,
            new Condition()
            {
                Name = "Bubble Beam",
                StartMessage = "is using bubble beaming",
                //lamda functions
                OnStart = (Animal animal) =>
                {
                    //Sleep for  a random no. of turns between 1-4
                    animal.StatusTime = Random.Range(1,5);
                    Debug.Log($"Will be bubble beamed for {animal.StatusTime} moves");
                },
                OnAfterTurn = (Animal animal) =>
                {
                   if(animal.StatusTime <= 0)
                   {
                       animal.CureStatus();
                       animal.StatusChanges.Enqueue($"{animal.Base.Name} has recovered from bubble beam!");
                    }

                    animal.StatusTime--;
                    animal.DecreaseHP(animal.MaxHp / 12);
                    animal.StatusChanges.Enqueue($"{animal.Base.Name} cannot breathe due to bubble beam");
                    
                }

            }

         }

        //Todo: add all others move that have status

    };

    public static float GetStatusBonus(Condition condition) 
    {
        if (condition == null) 
            return 1f;
        else if (condition.Id == ConditionID.slp || condition.Id == ConditionID.frz || condition.Id == ConditionID.sng 
            || condition.Id == ConditionID.bld)
            return 2f;
        else if (condition.Id == ConditionID.par || condition.Id == ConditionID.psn || condition.Id == ConditionID.brn 
            || condition.Id == ConditionID.slr || condition.Id == ConditionID.bbl)
            return 1.5f;

        return 1f;
    }
}

public enum ConditionID
{
    none,psn,brn,slp,par,frz,
    confusion,
    sng,bld,slr,bbl


    //par - 1/4 chance pokemon wont perform a move
}
