using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition 
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }

    public Action<Animal> OnStart { get; set; }
    public Func<Animal, bool> OnBeforeMove { get; set; } //use Func because we want to return a bool
    public Action<Animal> OnAfterTurn { get; set; } //use Action when we dont need to return a value
}
