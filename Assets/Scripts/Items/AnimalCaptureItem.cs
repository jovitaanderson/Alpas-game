using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items / Create new Animal Capture")]

public class AnimalCaptureItem : ItemBase
{
    [SerializeField] float catchRateModifier = 1;
    public override bool Use(Animal animal)
    {
        if (GameController.Instance.State == GameState.Battle)
            return true;

        return false;
    }

    public float CatchRateModifier => catchRateModifier; 
}
