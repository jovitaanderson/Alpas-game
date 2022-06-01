using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene, Paused}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    GameState state;
    GameState stateBeforePaused;


    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    public static GameController Instance {get; private set;}
    private void Awake()
    {
        Instance = this;
        ConditionsDB.Init();
    }


    private void Start()
    {
        //ToDO: remove this code aft jovita
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        //Todo: Remove code -> playerController.OnEnterTrainersView ...
        playerController.OnEnterTrainersView += (Collider2D trainerCollider) =>
        {
            var trainer = trainerCollider.GetComponentInParent<TrainerController>();
            if (trainer != null)
            {
                state = GameState.Cutscene;
                StartCoroutine(trainer.TriggerTrainerBattle(playerController));
            }
        };

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if(state == GameState.Dialog)
            state = GameState.FreeRoam;
        };
    }

    public void PausedGame(bool pause) 
    {
        if (pause)
        {
            stateBeforePaused = state;
            state = GameState.Paused;
        }
        else 
        {
            state = stateBeforePaused;
        }
    }

    public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        //We can get animalparty from plaayercontroller since they are both components of the player game object
        var playerParty = playerController.GetComponent<AnimalParty>();
        //FindObjectOfType<MapArea>() will get the reference and return the game object with MapArea
        var wildAnimal = CurrentScene.GetComponent<MapArea>().GetRandomWildAnimal();

        var wildAnimalCopy = new Animal(wildAnimal.Base, wildAnimal.Level);

        battleSystem.StartBattle(playerParty, wildAnimalCopy);
    }

    TrainerController trainer;
    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        this.trainer = trainer;
        var playerParty = playerController.GetComponent<AnimalParty>();
        var trainerParty = trainer.GetComponent<AnimalParty>();
        
        //todo: i not sure if this suppose to be here or not, pls check @shanice
        //var wildAnimalCopy = new Animal(wildAnimal.Base, wildAnimal.Level);

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    //Todo: aft jovita
    // public void OnEnterTrainersView(TrainerController trainer)
    // {
    //      state = GameState.CutScene;
    //      StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    // } 

    void EndBattle(bool won)
    {
        if(trainer != null && won == true)
        {
            trainer.BattleLost();
            trainer = null;
        }

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }

    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }
}
