using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//To create instances of this class in unity
[CreateAssetMenu(fileName = "Move", menuName ="Animal/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] new string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] AnimalType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp; //pp is the number of times a move can be performed

    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public AnimalType Type {
        get { return type; }
    }

    public int Power {
        get { return power; }
    }
    public int Accuracy {
        get { return accuracy; }
    }
    public int PP {
        get { return pp; }
    }
}
