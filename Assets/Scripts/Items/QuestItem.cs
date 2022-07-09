using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items / Create new quest item")]

public class QuestItem : ItemBase
{
    public override bool Use(Animal animal)
    {
        return false;
    }
}
