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

    Animal _animal;

    public void setData(Animal animal)
    {
        _animal = animal;
        nameText.text = animal.Base.Name;
        levelText.text = "Lvl " + animal.Level;
        hpBar.SetHP((float)animal.HP / animal.MaxHp);
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHPSmooth((float)_animal.HP / _animal.MaxHp);
    }
}
