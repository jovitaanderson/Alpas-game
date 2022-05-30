using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A way to create instances of the scriptableObject
//We add a attribute createasset menu, when we right-click on project tab
[CreateAssetMenu(fileName = "Animal", menuName = "Animal/Create new animal")]

/** Class to store Pokemon info such as stats and (learnable)moves 
 * 
 */
public class AnimalBase : ScriptableObject
{
    //Since we need to use this variable outside this class, to follow OOP we use SerializeField instead of public
    // as it is bad pratice

    [SerializeField] new string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] AnimalType type1;
    [SerializeField] AnimalType type2;

    //Base stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMove> learnableMoves;

    //Properties in C#
    public string Name {
        get { return name; }
    }
    public string Description {
        get { return description; }
    }
    public Sprite FrontSprite {
        get { return frontSprite; }
    }
    public Sprite BackSprite {
        get { return backSprite; }
    }
    public AnimalType Type1 {
        get { return type1; }
    }
    public AnimalType Type2 {
        get { return type2; }
    }
    public int MaxHp {
        get { return maxHp; }
    }
    public int Attack {
        get { return attack; }
    }
    public int Defense {
        get { return defense; }
    }
    public int SpAttack {
        get { return spAttack; }
    }
    public int SpDefense {
        get { return spDefense; }
    }
    public int Speed {
        get { return speed; }
    }
    public List<LearnableMove> LearnableMoves {
        get { return learnableMoves; }
    }

}

[System.Serializable] //Then it will appear in the inspect
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base {
        get { return moveBase; }
    }

    public int Level {
        get { return level;}
    }
}
public enum AnimalType
{
    None,
    Amphibian,
    Bird,
    Fish,
    Insect,
    Mammal,
    Reptile,
    Carnivore,
    Herbivore,
    Omnivore
}
public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    // These2are not actual stats,they're used to boost the moveAccuracy
    Accuracy,
    Evasion
}

public class TypeChart
{
    static float[][] chart =
    {
       /*                            AMP   BIR   FIS   INS   MAM   REP   CAR   HER   OMI*/
       /*Amphibians*/   new float[] {1f,   0.5f, 1f,   2f,   2f,   0.5f, 1f,   1f,   1f},
       /*Birds*/        new float[] {1f,   1f,   2f,   2f,   0.5f, 0.5f, 1f,   1f,   1f},
       /*Fish*/         new float[] {2f,   1f,   1f,   2f,   0.5f, 2f,   1f,   1f,   1f},
       /*Insects*/      new float[] {0.5f, 0.5f, 1f,   1f,   2f,   2f,   1f,   1f,   1f},
       /*Mammals*/      new float[] {1f,   2f,   2f,   0.5f, 1f,   1f,   1f,   1f,   1f},
       /*Reptiles*/     new float[] {2f,   2f,   0.5f, 0.5f, 1f,   1f,   1f,   1f,   1f},
       /*Carnivore*/    new float[] {1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   0.5f},
       /*Herbivore*/    new float[] {1f,   1f,   1f,   1f,   1f,   1f,   0.5f, 1f,   2f},
       /*Omivore*/      new float[] {1f,   1f,   1f,   1f,   1f,   1f,   2f,   0.5f, 1f}

    };
    public static float GetEffectiveness(AnimalType attackType, AnimalType defenseType)
    {
        if (attackType == AnimalType.None || defenseType == AnimalType.None)
            return 1;

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;
        return chart[row][col];
    }

}

