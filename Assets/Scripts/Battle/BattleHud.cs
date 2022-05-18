using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//To set the name and level text in the battleHud depending on the animal
public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;

    public void setData(Animal animal)
    {
        nameText.text = animal.Base.Name;
        levelText.text = "Lvl " + animal.Level;
        hpBar.SetHP((float)animal.HP / animal.MaxHp);
    }
}
