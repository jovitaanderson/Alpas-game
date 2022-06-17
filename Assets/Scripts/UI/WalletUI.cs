using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalletUI : MonoBehaviour
{
    [SerializeField] Text moneyText;

    private void Start()
    {
        SetMoneyText();
        Wallet.i.OnMoneyChanged += SetMoneyText;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        SetMoneyText();

    }

    public void Close() {
        gameObject.SetActive(false);
    }

    void SetMoneyText()
    {
        moneyText.text = "X " + Wallet.i.Money;
    }

}
