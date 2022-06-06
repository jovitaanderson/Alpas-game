using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> slots;
    [SerializeField] List<ItemSlot> animalCaptureSlots;
    [SerializeField] List<ItemSlot> tmSlots;

    List<List<ItemSlot>> allSlots;

    public event Action OnUpdated;

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>()
        {
            slots, animalCaptureSlots, tmSlots
        };
    }

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "ITEMS", "ANIMAL CAPTURE", "TMs & HMs"    
    };

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }

    public ItemBase UseItem(int itemIndex, Animal selectedAnimal)
    {
        var item = slots[itemIndex].Item;
        bool itemUsed = item.Use(selectedAnimal);
        if (itemUsed)
        {
            RemoveItem(item);
            return item;
        }

        return null;
    }

    public void RemoveItem(ItemBase item)
    {
        var itemSlot = slots.First(slot => slot.Item == item);
        itemSlot.Count--;
        if(itemSlot.Count == 0)
        {
            slots.Remove(itemSlot);
        }

        OnUpdated?.Invoke();
    }

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;
    public ItemBase Item => item;
    public int Count {
        get => count;
        set => count = value;
    }
}
