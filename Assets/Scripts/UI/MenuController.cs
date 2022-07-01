using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menu;

    public event Action<int> onMenuSelected;
    public event Action onBack;

    List<Text> menuItems;

    public int selectedItem = 0;


    private void Awake()
    {
        menuItems = menu.GetComponentsInChildren<Text>().ToList();
    }
    public void OpenMenu()
    {
        menu.SetActive(true);
        UpdateItemSelection();
    }
    public void CloseMenu()
    {
        menu.SetActive(false);
    }

    public void HandleUpdate()
    {
        int prevSelection = selectedItem;

        if (Input.GetKeyDown(SettingsManager.i.getKey("DOWN")) || Input.GetKeyDown(SettingsManager.i.getKey("DOWN1")))
            ++selectedItem;
        else if (Input.GetKeyDown(SettingsManager.i.getKey("UP")) || Input.GetKeyDown(SettingsManager.i.getKey("UP1")))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);

        if (prevSelection != selectedItem)
            UpdateItemSelection();

        //if press enter then go do action
        if (Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM")) || Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM1")))
        {
            Debug.Log(selectedItem);
            onMenuSelected?.Invoke(selectedItem);
            CloseMenu();
        }
        //else, if press escape, then go back
        else if (Input.GetKeyDown(SettingsManager.i.getKey("BACK")) || Input.GetKeyDown(SettingsManager.i.getKey("BACK1")))
        {
            onBack?.Invoke();
            CloseMenu();
        }
    }

    //TODO: implement method for mouse, this method has error
    /*
    public void MenuMouseSelect(int selectedItem)
    {
        this.selectedItem = selectedItem;
        UpdateItemSelection();
        onMenuSelected?.Invoke(selectedItem);
        CloseMenu();
    } */

    void UpdateItemSelection()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            if (i == selectedItem)
                menuItems[i].color = GlobalSettings.i.HighlightedColor;
            else
                menuItems[i].color = Color.black;
        }
    }
}
