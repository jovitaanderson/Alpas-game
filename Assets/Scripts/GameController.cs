using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    GameState state;

    private void Awake()
    {
        ConditionsDB.Init();
    }


    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        //We can get animalparty from plaayercontroller since they are both components of the player game object
        var playerParty = playerController.GetComponent<AnimalParty>();
        //FindObjectOfType<MapArea>() will get the reference and return the game object with MapArea
        var wildAnimal = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildAnimal();

        var wildAnimalCopy = new Animal(wildAnimal.Base, wildAnimal.Level);

        battleSystem.StartBattle(playerParty, wildAnimalCopy);
    }
    void EndBattle(bool won)
    {
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
    }
}
