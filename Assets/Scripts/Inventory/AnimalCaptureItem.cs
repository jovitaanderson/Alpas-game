using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items / Create new Animal Capture")]

public class AnimalCaptureItem : ItemBase
{
    public override bool Use(Animal animal)
    {
        return true;
    }
}
