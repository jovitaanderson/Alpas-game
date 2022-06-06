using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;

    Animal _animal;
    public void SetData(Animal animal)
    {
        _animal = animal;
        nameText.text = animal.Base.Name;
        levelText.text = "Lvl" + animal.Level;
        hpBar.SetHP((float)animal.HP / animal.MaxHp);
    }

    //Change color of selected party member
    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = GlobalSettings.i.HighlightedColor;
        else         
            nameText.color = Color.black;
    }

}
