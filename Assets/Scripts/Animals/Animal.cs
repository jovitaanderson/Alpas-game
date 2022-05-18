using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Calculte all stat values speific to a level
/** Class to store Pokemon actual info such as stats and actual moves 
 */
public class Animal
{
    public AnimalBase Base { get; set; } // put get and set  to access this properties outside the class
    public int Level { get; set; }

    public int HP { get; set; }
    public List<Move> Moves { get; set; }

    public Animal(AnimalBase pBase, int pLevel)
    {
        Base = pBase;
        Level = pLevel;
        HP = MaxHp;

        //Will add moves to the Animal if level is reached [Animal can only have 4 moves]
        Moves = new List<Move>();
        foreach(var move in Base.LearnableMoves)
        {
            if (move.Level <= Level) { Moves.Add(new Move(move.Base)); }
            if (Moves.Count >= 4) { break; }
        }
    }

    //Properites
    public int Attack {
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; } //this is the forumla used in Pokemon
    }
    public int Defense {
        get { return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5; }
    }
    public int SpAttack {
        get { return Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5; }
    }
    public int SpDefense {
        get { return Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5; }
    }
    public int Speed {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; }
    }
    public int MaxHp{
        get { return Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10; }
    }
}
