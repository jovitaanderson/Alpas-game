using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//teleports the player to a different position without switching scenes
public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] DestinatonIdentifier destinationPortal;
    [SerializeField] Transform spawnPoint;

    PlayerController player;
   public void OnPlayerTriggered(PlayerController player) 
   {
       player.Character.Animator.IsMoving = false;
       this.player = player;
       StartCoroutine(Teleport());
   }

   Fader fader;

   private void Start()
   {
       fader = FindObjectOfType<Fader>();
   }

   IEnumerator Teleport() 
   {

       GameController.Instance.PausedGame(true);
       yield return fader.FadeIn(0.5f);

        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.Instance.PausedGame(false);

   }

   public Transform SpawnPoint => spawnPoint;
}
