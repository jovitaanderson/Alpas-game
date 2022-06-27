using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureChestSelectController : MonoBehaviour
{
    [SerializeField] GameObject selectChest;

    public event Action<int> onChestSelected;
    public event Action onBack;

    GameObject[] chestTypes;

    public int selectedItem = 0;


    private void Awake()
    {
        chestTypes = selectChest.GetComponentsInChildren<GameObject>();
    }
    public void OpenMenu()
    {
        selectChest.SetActive(true);
        UpdateItemSelection();
    }
    public void CloseMenu()
    {
        selectChest.SetActive(false);
    }

    public void HandleUpdate()
    {
        int prevSelection = selectedItem;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, chestTypes.Length - 1);

        if (prevSelection != selectedItem)
            UpdateItemSelection();

        //if press enter then go do action
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log(selectedItem);
            onChestSelected?.Invoke(selectedItem);
            CloseMenu();
        }
        //else, if press escape, then go back
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            onBack?.Invoke();
            CloseMenu();
        }
    }

    void UpdateItemSelection()
    {
        for (int i = 0; i < chestTypes.Length; i++)
        {
            if (i == selectedItem)
                chestTypes[i].GetComponentInChildren<Text>().color = GlobalSettings.i.HighlightedColor;
            else
                chestTypes[i].GetComponentInChildren<Text>().color = Color.black;
        }
    }
}
