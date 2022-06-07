using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory {  Items, AnimalCapture, Tms }

public class Inventory : MonoBehaviour, ISavable
{
    [SerializeField] List<ItemSlot> slots;
    [SerializeField] List<ItemSlot> animalCaptureSlots;
    [SerializeField] List<ItemSlot> tmSlots; //we are not using tm(?)

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
        "ITEMS", "ANIMAL CAPTURE", "TMs & HMs"     //we are not using tm(?)
    };

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }

    public ItemBase UseItem(int itemIndex, Animal selectedAnimal, int selectedCategory)
    {
        var currentSlots = GetSlotsByCategory(selectedCategory);
        var item = currentSlots[itemIndex].Item;
        bool itemUsed = item.Use(selectedAnimal);
        if (itemUsed)
        {
            RemoveItem(item);
            return item;
        }

        return null;
    }

    public void AddItem(ItemBase item, int count=1)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
        if (itemSlot != null)
        {
            itemSlot.Count += count;
        }
        else
        {
            currentSlots.Add(new ItemSlot()
            {
                Item = item,
                Count = count
            });
        }

        OnUpdated?.Invoke();
    }

    public void RemoveItem(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);
        var itemSlot = currentSlots.First(slot => slot.Item == item);
        itemSlot.Count--;
        if(itemSlot.Count == 0)
        {
            currentSlots.Remove(itemSlot);
        }

        OnUpdated?.Invoke();
    }

    public bool HasItem(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        return currentSlots.Exists(slot => slot.Item == item);
    }

    ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if (item is RecoveryItem)
            return ItemCategory.Items;
        else if (item is AnimalCaptureItem)
            return ItemCategory.AnimalCapture;
        else
            return ItemCategory.Tms; //Todo: remove this if tms is not used
    }

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public object CaptureState()
    {
        var saveData = new InventorySaveData()
        {
            items = slots.Select(i => i.GetSaveData()).ToList(),
            animalCaptures = animalCaptureSlots.Select(i => i.GetSaveData()).ToList(),
            tms = tmSlots.Select(i => i.GetSaveData()).ToList() //we are not using tm(?)
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as InventorySaveData;
        slots = saveData.items.Select(i => new ItemSlot(i)).ToList();
        animalCaptureSlots = saveData.animalCaptures.Select(i => new ItemSlot(i)).ToList();
        tmSlots = saveData.tms.Select(i => new ItemSlot(i)).ToList(); //we are not using tm(?)

        allSlots = new List<List<ItemSlot>>() { slots, animalCaptureSlots, tmSlots };

        OnUpdated?.Invoke();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemSlot()
    {

    }

    //construtor
    public ItemSlot (ItemSaveData saveData)
    {
        item = ItemDB.GetItemByName(saveData.name);
        count = saveData.count;
    }

    //Function to convert the item slot to itemsavedata
    public ItemSaveData GetSaveData()
    {
        var saveData = new ItemSaveData()
        {
            name = item.Name,
            count = count
        };

        return saveData;
    }
    public ItemBase Item {
        get => item;
        set => item = value;
    }
    public int Count {
        get => count;
        set => count = value;
    }
}

[Serializable]
public class ItemSaveData
{
    public string name;
    public int count;
}

[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items;
    public List<ItemSaveData> animalCaptures;
    public List<ItemSaveData> tms;

}
