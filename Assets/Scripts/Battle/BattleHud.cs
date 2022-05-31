using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

//To set the name and level text in the battleHud depending on the animal
public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;

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
        SetExp();

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

    public void SetExp() 
    {
        //enemy dosent have exp bar
        if (expBar == null) return;

        float normalisedExp = GetNormalisedExp();
        expBar.transform.localScale = new Vector3(normalisedExp, 1, 1);
    }

    public IEnumerator SetExpSmooth() 
    {
        //enemy dosent have exp bar
        if (expBar == null) yield break;

        float normalisedExp = GetNormalisedExp();
        yield return expBar.transform.DOScaleX(normalisedExp, 1.5f).WaitForCompletion();
    }

    float GetNormalisedExp()
    {
        int currLevelExp = _animal.Base.GetExpForLevel(_animal.Level);
        int nextLevelExp = _animal.Base.GetExpForLevel(_animal.Level + 1);
        float normalisedExp = (float)(_animal.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalisedExp);
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
