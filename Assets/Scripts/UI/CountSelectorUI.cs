using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountSelectorUI : MonoBehaviour
{
    [SerializeField] Text countTxt;
    [SerializeField] Text priceTxt;
    bool selected;
    bool exit;
    int currentCount;
    int maxCount;
    float pricePerUnit;
    public IEnumerator ShowSelector(int maxCount, float pricePerUnit, Action<int> onCountSelected)
    {
        this.maxCount = maxCount;
        this.pricePerUnit = pricePerUnit;

        selected = false;
        exit = false;
        currentCount = 1;
        gameObject.SetActive(true);
        SetValues();
        yield return new WaitUntil(( ) => selected || exit == true);

        if (selected)
            onCountSelected?.Invoke(currentCount);
        else
            onCountSelected?.Invoke(-1);


        gameObject.SetActive(false);


    }

    private void Update()
    {
        int prevCount = currentCount;

        if (Input.GetKeyDown(KeybindManager.i.keys["UP"]))
            ++currentCount;
        else if (Input.GetKeyDown(KeybindManager.i.keys["Down"]))
            --currentCount;

        currentCount = Mathf.Clamp(currentCount, 1, maxCount);

        if(currentCount != prevCount)
            SetValues();


        if (Input.GetKeyDown(KeybindManager.i.keys["CONFIRM"]))
            selected = true;

        if (Input.GetKeyDown(KeybindManager.i.keys["BACK"]))
            exit = true;


    }

    void SetValues()
    {
        countTxt.text = "x " + currentCount;
        priceTxt.text = "$ " + pricePerUnit * currentCount;
    }
}
