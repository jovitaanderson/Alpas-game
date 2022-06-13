using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] new string name;
    [SerializeField] string description;
    [SerializeField] string usedMessage;
    [SerializeField] Sprite icon;
    [SerializeField] float price;
    [SerializeField] bool isSellable;
    public string Name => name;
    public string Description => description;
    public string UsedMessage => usedMessage;
    public Sprite Icon => icon;

    public float Price => price;
    public bool IsSellable => isSellable;

    public virtual bool Use(Animal animal)
    {
        return false;
    }

    //TODO: remove if not used
    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutsideBattle => true;
}
