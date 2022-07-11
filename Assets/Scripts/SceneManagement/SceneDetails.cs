using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;
    [SerializeField] string sceneMusic;
    [SerializeField] Sprite backgroundBattle;
    [SerializeField] Sprite backgroundCirclesBattle;
    //[SerializeField] Image backgroundBattleImage;
    //[SerializeField] Image backgroundBattleCirclesImage1;
    //[SerializeField] Image backgroundBattleCirclesImage2;

    BattleSystem battleSystem;

    public bool IsLoaded { get; private set; }
    List<SavableEntity> savableEntities;

   private void OnTriggerEnter2D(Collider2D collision)
   {
       if (collision.tag == "Player")
       {
           Debug.Log($"Entered {gameObject.name}");

           LoadScene();
           GameController.Instance.SetCurrentScene(this);

            //play the music of the scene
            if (sceneMusic != null)
                AudioManager.i.Play(sceneMusic, fade: true);

            //change background battle
            if (backgroundBattle != null)
                battleSystem.GrassBackground(backgroundBattle);


            
            //change background circles battle
            if (backgroundCirclesBattle != null)
            {
                battleSystem.GrassBackground(backgroundCirclesBattle);
            }
                

            //Load alll connected scenes
            foreach (var scene in connectedScenes)
           {
               scene.LoadScene();
           }

            //Unload the scenens that are not connected anymore
            var prevScene = GameController.Instance.PrevScene;
           if (prevScene != null)
           {
               var previouslyLoadedScenes = prevScene.connectedScenes;
               foreach (var scene in previouslyLoadedScenes)
               {
                   if (!connectedScenes.Contains(scene) && scene != this)
                   {
                       scene.UnloadScene();
                   }
               }

               if(!connectedScenes.Contains(prevScene))
                prevScene.UnloadScene();
           }
       }
   }

   public void LoadScene()
   {
        if (!IsLoaded)
        {
            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;

            operation.completed += (AsyncOperation op) =>
            {
                //These lines will run once var operation is fully completed
                savableEntities = GetSavableEntitiesInScene();
                SavingSystem.i.RestoreEntityStates(savableEntities);
            };


        }
   }
   public void UnloadScene()
   {
        if (IsLoaded)
        {
            SavingSystem.i.CaptureEntityStates(savableEntities);

            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;

            savableEntities = GetSavableEntitiesInScene();
        }
   }

    List<SavableEntity> GetSavableEntitiesInScene()
    {
        var currScene = SceneManager.GetSceneByName(gameObject.name);
        var saveableEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currScene).ToList();
        return saveableEntities;
    }

    public string SceneMusic => sceneMusic;
}
