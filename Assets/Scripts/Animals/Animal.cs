using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Calculte all stat values speific to a level
/** Class to store Pokemon actual info such as stats and actual moves 
 */
[System.Serializable] //classes will only be shown in the inspector if we use this arttibute 
public class Animal
{
    [SerializeField] AnimalBase _base;
    [SerializeField] int level;

    public AnimalBase Base {
        get { return _base; }
    } 
    public int Level { 
        get {  return level;}
    }

    public int HP { get; set; }
    public List<Move> Moves { get; set; }

    public void Init()
    {
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

    public DamageDetails TakeDamage(Move move, Animal attacker)
    {
        //critical hits
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
        {
            critical = 1.5f;
        }


        //type effectivness value depending on animal type
        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        //conditional operator
        float attack = (move.Base.IsSpecial) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.IsSpecial) ? SpDefense : Defense;


        //Forumla used to calculate damage taken in pokemon
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }

        return damageDetails;
     }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

}
public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }

}
