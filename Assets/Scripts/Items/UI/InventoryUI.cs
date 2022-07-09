using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, Busy}

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Text categoryText;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    [SerializeField] PartyScreen partyScreen;

    Action<ItemBase> onItemUsed;

    int selectedItem = 0;
    int selectedCategory = 0;
    InventoryUIState state;

    //No. of items that can be seen in the bag UI
    const int itemsInViewport = 6;

    List<ItemSlotUI> slotUIList;
    Inventory inventory;
    RectTransform itemListRect;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();
        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
        // Clear all the existing items
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }

    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed = null)
    {
        this.onItemUsed = onItemUsed;

        if (state == InventoryUIState.ItemSelection) 
        {
            int prevSelection = selectedItem;
            int prevCategory = selectedCategory;

            if (Input.GetKeyDown(SettingsManager.i.getKey("DOWN")) || Input.GetKeyDown(SettingsManager.i.getKey("DOWN1")))
                ++selectedItem;
            else if (Input.GetKeyDown(SettingsManager.i.getKey("UP")) || Input.GetKeyDown(SettingsManager.i.getKey("UP1")))
                --selectedItem;
            else if (Input.GetKeyDown(SettingsManager.i.getKey("RIGHT")) || Input.GetKeyDown(SettingsManager.i.getKey("RIGHT1")))
                ++selectedCategory;
            else if (Input.GetKeyDown(SettingsManager.i.getKey("LEFT")) || Input.GetKeyDown(SettingsManager.i.getKey("LEFT1")))
                --selectedCategory;

            if (selectedCategory > Inventory.ItemCategories.Count - 1)
            {
                selectedCategory = 0;
            }
            else if (selectedCategory < 0)
            {
                selectedCategory = Inventory.ItemCategories.Count - 1;
            }
            //selectedCategory = Mathf.Clamp(selectedCategory, 0, Inventory.ItemCategories.Count - 1);
            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);
            

            if (prevCategory!=selectedCategory)
            {
                ResetSelection();
                categoryText.text = Inventory.ItemCategories[selectedCategory];
                UpdateItemList();
            }
            else if (prevSelection != selectedItem)
            {
                UpdateItemSelection();
            }
               

            //open party screen if press enter
            if (Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM")) || Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM1")))
                StartCoroutine(ItemSelected());

            //if press escape, close the bag
            else if (Input.GetKeyDown(SettingsManager.i.getKey("BACK")) || Input.GetKeyDown(SettingsManager.i.getKey("BACK1")))
                onBack?.Invoke();
        }
        else if (state == InventoryUIState.PartySelection)
        {
            //Handle party selection
            Action onSelected = () =>
            {
                //Use the item on the selected animal
                StartCoroutine(UseItem());

            };
            Action onBackPartyScreen = () =>
            {
                ClosePartyScreen();
            };
            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
    }

    IEnumerator ItemSelected()
    {
        //check this again if got any issues (remove if not used)
        state = InventoryUIState.Busy;

        var item = inventory.GetItem(selectedItem, selectedCategory);

        if (GameController.Instance.State == GameState.Shop)
        {
            onItemUsed?.Invoke(item);
            state = InventoryUIState.ItemSelection;
            yield break;
        }

        //Todo: check this again if got any issues (remove if not used)
        if (GameController.Instance.State == GameState.Battle)
        {
            //in battle
            if (!item.CanUseInBattle)
            {
                if(selectedCategory == (int)ItemCategory.QuestItems)
                    yield return DialogManager.Instance.ShowDialogText($"Quest items cannot be used");
                else
                    yield return DialogManager.Instance.ShowDialogText($"This item cannot be used in battle");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            //outside battle
            if (!item.CanUseOutsideBattle)
            {
                if (selectedCategory == (int)ItemCategory.QuestItems)
                    yield return DialogManager.Instance.ShowDialogText($"Quest items cannot be used");
                else
                    yield return DialogManager.Instance.ShowDialogText($"This item cannot be used outside of battle");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }

        //if curr selected is pokeball/quest item
        if (selectedCategory == (int)ItemCategory.AnimalCapture || selectedCategory == (int)ItemCategory.QuestItems)
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();
        }
    }

    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        var item = inventory.GetItem(selectedItem, selectedCategory);
        var animal = partyScreen.SelectedMember;

        //Handle Evolution Items
        if (item is EvolutionItem)
        {
            var evolution = animal.CheckForEvolution(item);
            if (evolution != null)
            {
                yield return EvolutionManager.i.Evolve(animal, evolution);
            }
            else
            {
                yield return DialogManager.Instance.ShowDialogText($"It won't have any affect!");
                ClosePartyScreen();
                yield break;
            }
        }

        
        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if (usedItem != null)
        {
            if (usedItem is RecoveryItem)
                yield return DialogManager.Instance.ShowDialogText($"The player used {usedItem.Name}"); //TODO: change the player to player.name, add on to {usedItem.UsedMessage}
            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            if (selectedCategory == (int)ItemCategory.Items)
                yield return DialogManager.Instance.ShowDialogText($"It won't have any affect!");
        }
        ClosePartyScreen();
        
       
    }

    

    void UpdateItemSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);

        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }


        if (slots.Count > 0)
        {
            var item = slots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }
        

        HandleScrolling();
    }

    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewport) return;

        float scrollPos = Mathf.Clamp( selectedItem - itemsInViewport/2, 0 , selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x , scrollPos );

        bool showUpArrow = selectedItem > itemsInViewport / 2;
        upArrow.gameObject.SetActive(showUpArrow);
        bool showDownArrow = selectedItem + itemsInViewport / 2 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);

    }

    void ResetSelection()
    {
        selectedItem = 0;

        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;

        itemDescription.text = "";
    }

    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
    }

    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;
        partyScreen.gameObject.SetActive(false);
    }

   
}
