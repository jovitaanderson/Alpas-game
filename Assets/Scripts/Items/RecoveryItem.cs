using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items / Create new recovery item")]
//inheriting from ItemBase
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;

    [Header("PP")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;

    [Header("Status Conditions")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllStatus;

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    public override bool Use(Animal animal)
    {
        //Revive
        if(revive || maxRevive)
        {
            if (animal.HP > 0)
                return false;

            if (revive)
                animal.IncreaseHP(animal.MaxHp / 2);
            else if (maxRevive)
                animal.IncreaseHP(animal.MaxHp);

            animal.CureStatus();

            return true;
        }

        //No other items can be used on fainted animal
        if (animal.HP == 0)
            return false;

        //Restore HP
        if(restoreMaxHP || hpAmount > 0)
        {
            if (animal.HP == animal.MaxHp)
                return false;

            if (restoreMaxHP)
                animal.IncreaseHP(animal.MaxHp);
            else
                animal.IncreaseHP(hpAmount);
        }

        //Recover Status
        if (recoverAllStatus || status != ConditionID.none)
        {
            if (animal.Status == null && animal.VolatileStatus == null)
                return false;

            if (recoverAllStatus)
            {
                animal.CureStatus(); ;
                animal.CureVolatileStatus();
            }
            else
            {
                if (animal.Status.Id == status)
                    animal.CureStatus();
                else if (animal.VolatileStatus.Id == status)
                    animal.CureVolatileStatus();
                else
                    return false;
            }
        }

        //Restore PP
        if (restoreMaxHP)
        {
            bool canUseItem = false;
            foreach (var move in animal.Moves)
            {
                if (move.PP != move.Base.PP)
                    canUseItem = true;
            }
            if (canUseItem)
                animal.Moves.ForEach(m => m.IncreasePP(m.Base.PP));
            else
                return false;
        }
        else if (ppAmount > 0)
        {
            bool canUseItem = false;
            foreach (var move in animal.Moves)
            {
                if (move.PP != move.Base.PP)
                    canUseItem = true;
            }
            if (canUseItem)
                animal.Moves.ForEach(m => m.IncreasePP(ppAmount));
            else
                return false;
        }

        return true;
    }
}
