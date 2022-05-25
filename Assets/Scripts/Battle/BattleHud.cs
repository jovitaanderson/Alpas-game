using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//To set the name and level text in the battleHud depending on the animal
public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;

    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    Animal _animal;
    Dictionary<ConditionID, Color> statusColors;

    public void SetData(Animal animal)
    {
        _animal = animal;
        nameText.text = animal.Base.Name;
        levelText.text = "Lvl " + animal.Level;
        hpBar.SetHP((float)animal.HP / animal.MaxHp);

        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.psn, psnColor },
            {ConditionID.brn, brnColor },
            {ConditionID.slp, slpColor },
            {ConditionID.par, parColor },
            {ConditionID.frz, frzColor },
        };

        SetStatusText();
        _animal.OnStatusChanged += SetStatusText;
    }

    void SetStatusText()
    {
        if (_animal.Status == null) {
            statusText.text = "";
        }
        else
        {
            statusText.text=_animal.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_animal.Status.Id];
        }
    }

    public IEnumerator UpdateHP()
    {
        if (_animal.HpChanged == true)
        {
            yield return hpBar.SetHPSmooth((float)_animal.HP / _animal.MaxHp);
            _animal.HpChanged = false;
        }
    }
}
