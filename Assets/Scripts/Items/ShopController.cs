using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopState { Menu, Buying, Selling, Busy }

public class ShopController : MonoBehaviour
{
    //script to put all the code to buy and sell items
    [SerializeField] Vector2 shopCameraOffset;
    [SerializeField] Vector2 walletUIOffset;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] ShopUI shopUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelectorUI;
    [SerializeField] GameObject miniMapWindow;

    public event Action OnStart;
    public event Action OnFinish;

    ShopState state;

    Merchant merchant;

    public static ShopController i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    Inventory inventory;
    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    public IEnumerator StartTrading(Merchant merchant)
    {
        this.merchant = merchant;
        OnStart?.Invoke();
        yield return StartMenuState();
    }

    IEnumerator StartMenuState()
    {
        state = ShopState.Menu;

        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText("How may I serve you?",
            waitForInput: false,
            choices: new List<string>() { "Buy", "Sell", "Quit" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            //buy
            yield return GameController.Instance.MoveCamera(shopCameraOffset);
            walletUI.transform.position += new Vector3(walletUIOffset.x, walletUIOffset.y);
            miniMapWindow.SetActive(false);

            //walletUI.Show();

            shopUI.Show(merchant.AvaiableItems, (item) => StartCoroutine(BuyItem(item)),
                () => StartCoroutine(OnBackFromBuying()));

            state = ShopState.Buying;
        }
        else if (selectedChoice == 1)
        {
            //sell
            walletUI.transform.position += new Vector3(walletUIOffset.x, walletUIOffset.y);
            miniMapWindow.SetActive(false);

            state = ShopState.Selling;
            inventoryUI.gameObject.SetActive(true);

        }
        else if (selectedChoice == 2)
        {
            //quit
            OnFinish?.Invoke();
            yield break;
        }
    }

    public void HandleUpdate()
    {
        if (state == ShopState.Selling)
        {
            inventoryUI.HandleUpdate(OnBackFromSelling, (selectedItem) => StartCoroutine(SellItem(selectedItem)));
        }
        else if(state == ShopState.Buying)
        {
            shopUI.HandleUpdate();
        }
    }

    void OnBackFromSelling()
    {
        walletUI.transform.position -= new Vector3(walletUIOffset.x, walletUIOffset.y);
        miniMapWindow.SetActive(true);
        inventoryUI.gameObject.SetActive(false);
        StartCoroutine(StartMenuState());
    }

    IEnumerator SellItem(ItemBase item)
    {

        state = ShopState.Busy;

        if (!item.IsSellable)
        {
            yield return DialogManager.Instance.ShowDialogText("You cannot sell that!");
            state = ShopState.Selling;
            yield break;
        }

        //walletUI.Show();

        float sellingPrice = Mathf.Round(item.Price / 2);
        int countToSell = 1;

        int itemCount = inventory.GetItemCount(item);
        if(itemCount > 1)
        {
            yield return DialogManager.Instance.ShowDialogText($"How many would you like to sell?",
                waitForInput: false, autoClose: false); 

            yield return countSelectorUI.ShowSelector(itemCount, sellingPrice,
                (selectedCount) => countToSell = selectedCount);

            DialogManager.Instance.CloseDialog(); 

        }
        sellingPrice = sellingPrice * countToSell;

        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText($"I can give {sellingPrice} for that! Would you like to sell?",
            waitForInput: false,
            choices: new List<string>() { "Yes", "No" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            //yes -> sell the item
            inventory.RemoveItem(item, countToSell);
            //add selling price to player's wallet
            Wallet.i.AddMoney(sellingPrice);
            yield return DialogManager.Instance.ShowDialogText($"Sold {item.Name} and received {sellingPrice}!");
        }

        //walletUI.Close();

        state = ShopState.Selling;
    }

    IEnumerator BuyItem(ItemBase item)
    {
        state = ShopState.Busy;
        yield return DialogManager.Instance.ShowDialogText("How many would you like to buy ? ",
        waitForInput: false, autoClose: false);
        int countToBuy = 1;
        yield return countSelectorUI.ShowSelector(100, item.Price,
        (selectedCount) => countToBuy = selectedCount);
        DialogManager.Instance.CloseDialog();
        if (countToBuy > 0)
        {
            float totalPrice = item.Price * countToBuy;

            if (Wallet.i.HasMoney(totalPrice))
            {
                int selectedChoice = 0;
                yield return DialogManager.Instance.ShowDialogText($"That will be {totalPrice}",
                    waitForInput: false,
                    choices: new List<string>() { "Yes", "No" },
                    onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

                if (selectedChoice == 0)
                {
                    //selected yes
                    inventory.AddItem(item, countToBuy);
                    Wallet.i.TakeMoney(totalPrice);
                    yield return DialogManager.Instance.ShowDialogText("Thank you for shopping with us!");
                }
            }
            else
            {
                yield return DialogManager.Instance.ShowDialogText(" Not enough money for that ! ");
            }
        }

        state = ShopState.Buying;
    }

    IEnumerator OnBackFromBuying()
    {

        yield return GameController.Instance.MoveCamera(-shopCameraOffset);
        walletUI.transform.position -= new Vector3(walletUIOffset.x, walletUIOffset.y);
        miniMapWindow.SetActive(true);
        shopUI.Close();
        //walletUI.Close();
        StartCoroutine(StartMenuState());
    }
}
