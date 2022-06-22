using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory {  Items, AnimalCapture } //, Tms

public class Inventory : MonoBehaviour, ISavable
{
    [SerializeField] List<ItemSlot> slots;
    [SerializeField] List<ItemSlot> animalCaptureSlots;
    

    List<List<ItemSlot>> allSlots;

    public event Action OnUpdated;

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>()
        {
            slots, animalCaptureSlots
        };
    }

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "ITEMS", "ANIMAL CAPTURE"
    };

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }

    public ItemBase GetItem(int itemIndex, int categoryIndex)
    {
        var currentSlots = GetSlotsByCategory(categoryIndex);
        return currentSlots[itemIndex].Item;
    }

    public ItemBase UseItem(int itemIndex, Animal selectedAnimal, int selectedCategory)
    {
        var item = GetItem(itemIndex, selectedCategory);
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

    public int GetItemCount(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);

        if (itemSlot != null)
            return itemSlot.Count;
        else
            return 0;
        
    }

    public void RemoveItem(ItemBase item, int countToRemove = 1)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
        itemSlot.Count -= countToRemove;
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
        if (item is RecoveryItem || item is EvolutionItem)
            return ItemCategory.Items;
        else 
            return ItemCategory.AnimalCapture;
        
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
            
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as InventorySaveData;
        slots = saveData.items.Select(i => new ItemSlot(i)).ToList();
        animalCaptureSlots = saveData.animalCaptures.Select(i => new ItemSlot(i)).ToList();
        

        allSlots = new List<List<ItemSlot>>() { slots, animalCaptureSlots };

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
        item = ItemDB.GetObjectByName(saveData.name);
        count = saveData.count;
    }

    //Function to convert the item slot to itemsavedata
    public ItemSaveData GetSaveData()
    {
        var saveData = new ItemSaveData()
        {
            name = item.name,
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
    
}
