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
    public void Init(Animal animal)
    {
        _animal = animal;
        UpdateData();

        _animal.OnHPChanged += UpdateData;

    }

    void UpdateData()
    {
        nameText.text = _animal.Base.Name;
        levelText.text = "Lvl" + _animal.Level;
        hpBar.SetHP((float)_animal.HP / _animal.MaxHp);
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
