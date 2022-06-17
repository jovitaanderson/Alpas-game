using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class coinsManager : MonoBehaviour
{
    public static coinsManager instance;
    public Text text;
    int money;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ChangeMoney(int coinValue)
    {
        money += coinValue;
        text.text = "X " + money.ToString();
    }
}
