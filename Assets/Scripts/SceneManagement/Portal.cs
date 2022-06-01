using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] DestinatonIdentifier destinationPortal;
    [SerializeField] Transform spawnPoint;

    PlayerController player;
   public void OnPlayerTriggered(PlayerController player) 
   {
       player.Character.Animator.IsMoving = false;
       this.player = player;
       StartCoroutine(SwitchScene());
   }

   Fader fader;

   private void Start()
   {
       fader = FindObjectOfType<Fader>();
   }

   IEnumerator SwitchScene() 
   {
       DontDestroyOnLoad(gameObject);

       GameController.Instance.PausedGame(true);
       yield return fader.FadeIn(0.5f);

       yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.Instance.PausedGame(false);

        Destroy(gameObject);
   }

   public Transform SpawnPoint => spawnPoint;
}

public enum DestinatonIdentifier {A,B,C,D,E}
