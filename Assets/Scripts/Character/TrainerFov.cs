using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFov : MonoBehaviour, IPlayerTriggerable
{
   public void OnPlayerTriggered(PlayerController playerController)
   {
       playerController.Character.Animator.IsMoving = false;
       GameController.Instance.OnEnterTrainersView(GetComponentInParent<TrainerController>());
   }
}
