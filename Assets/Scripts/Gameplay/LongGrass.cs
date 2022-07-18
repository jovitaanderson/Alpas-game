using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{

    public void OnPlayerTriggered(PlayerController playerController)
    {
        if (UnityEngine.Random.Range(1, 101) <= 15) 
        {
            playerController.Character.Animator.IsMoving = false;
            GameController.Instance.StartBattle(BattleTrigger.LongGrass);
        }
    }

    public bool TriggerRepeatedly => true;
}
