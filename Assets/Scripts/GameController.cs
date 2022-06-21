using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Menu, PartyScreen, Bag, Cutscene, Paused, Evolution, Shop, Instructions, TreasureChest}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

    [SerializeField] GameObject miniMapWindow;
    [SerializeField] GameObject walletUI;
    [SerializeField] GameObject instructionsPanel;

    GameState state;
    GameState prevState;
    GameState stateBeforeEvolution;


    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    MenuController menuController;

    public static GameController Instance {get; private set;}
    private void Awake()
    {
        Instance = this;

        menuController = GetComponent<MenuController>();

        //TODO: Uncomment this if we want to remove mouse from game
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        AnimalDB.Init();
        MoveDB.Init();
        ConditionsDB.Init();
        ItemDB.Init();
        QuestDB.Init();
    }


    private void Start()
    {
        if (MainController.checkLoadGame == true)
        {
            string savedSlotName = PlayerPrefs.GetString("SavedGame");
            SavingSystem.i.Load(savedSlotName);
            state = GameState.FreeRoam; //todo: remove this line(?)
        }
        else 
        {
            PlayerPrefs.SetString("SavedGame", "saveSlot1");
            SavingSystem.i.Save("saveSlot1");
        }

        battleSystem.OnBattleOver += EndBattle;

        partyScreen.Init();

        DialogManager.Instance.OnShowDialog += () =>
        {
            prevState = state;
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnDialogFinished += () =>
        {
            if(state == GameState.Dialog)
                state = prevState;
        };

        menuController.onBack += () =>
        {
            state = GameState.FreeRoam;
        };

        menuController.onMenuSelected += OnMenuSelected;

        EvolutionManager.i.OnStartEvolution += () =>
        {
            stateBeforeEvolution = state;
            state = GameState.Evolution;
        };
        EvolutionManager.i.OnCompleteEvolution += () =>
        {
            partyScreen.SetPartyData();
            state = stateBeforeEvolution;

            AudioManager.i.PlayMusic(CurrentScene.SceneMusic, fade: true);
        };

        TreasureChestManager.i.OnStartTreasureChest += () => state = GameState.TreasureChest;
        TreasureChestManager.i.OnCompleteTreasureChest += () => state = GameState.FreeRoam;

        ShopController.i.OnStart += () => state = GameState.Shop;
        ShopController.i.OnFinish += () => state = GameState.FreeRoam;
    }

    public void PauseGame(bool pause) 
    {
        if (pause)
        {
            prevState = state;
            state = GameState.Paused;
        }
        else 
        {
            state = prevState;
        }
    }

    public void StartBattle()
    {   
        //We can get animalparty from plaayercontroller since they are both components of the player game object
        var playerParty = playerController.GetComponent<AnimalParty>();
        //FindObjectOfType<MapArea>() will get the reference and return the game object with MapArea
        var wildAnimal = CurrentScene.GetComponent<MapArea>().GetRandomWildAnimal();
        var wildAnimalCopy = new Animal(wildAnimal.Base, wildAnimal.Level);

        if (playerParty.GetHealthyAnimal() == null)
        {
            StartCoroutine(DialogManager.Instance.ShowDialogText("All your animals are fainted, cannot fight wild animals."));
        }
        else
        {
            instructionsPanel.SetActive(false);
            state = GameState.Battle;
            miniMapWindow.SetActive(false);
            walletUI.SetActive(false);
            battleSystem.gameObject.SetActive(true);
            worldCamera.gameObject.SetActive(false);

            battleSystem.StartBattle(playerParty, wildAnimalCopy);
        }
    }

    TrainerController trainer;
    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        miniMapWindow.SetActive(false);
        walletUI.SetActive(false);
        walletUI.SetActive(false);
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        this.trainer = trainer;
        var playerParty = playerController.GetComponent<AnimalParty>();
        var trainerParty = trainer.GetComponent<AnimalParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    public void OnEnterTrainersView(TrainerController trainer)
    {
        var playerParty = playerController.GetComponent<AnimalParty>();
        if (playerParty.GetHealthyAnimal() == null)
        {
            StartCoroutine(DialogManager.Instance.ShowDialogText("All your animals are fainted, cannot fight with trainer."));
        } else
        {
            state = GameState.Cutscene;
            StartCoroutine(trainer.TriggerTrainerBattle(playerController));
        }

    } 

    void EndBattle(bool won)
    {
        if(trainer != null && won == true)
        {
            trainer.BattleLost();
            trainer = null;
        }

        partyScreen.SetPartyData();

        state = GameState.FreeRoam;
        miniMapWindow.SetActive(true);
        walletUI.SetActive(true);
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);


        var playerParty = playerController.GetComponent<AnimalParty>();
        //changed abit from the tutorial because we dunnid evolve straight away after a battle
        //var hasEvolutions = playerParty.CheckForEvolutions();
        //if (hasEvolutions)
        AudioManager.i.PlayMusic(CurrentScene.SceneMusic, fade: true);
        StartCoroutine(playerParty.RunEvolutions());
        //else
            
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();

            //if user press M key, call menu controller. open menu function
            if (Input.GetKeyDown(KeyCode.M))
            {
                playerController.Character.Animator.IsMoving = false;
                menuController.OpenMenu();
                state = GameState.Menu;
            }

        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if (state == GameState.Menu)
        {
            menuController.HandleUpdate();
        }
        else if (state == GameState.PartyScreen)
        {
            Action onSelected = () =>
            {
                //Todo: Go to Summary Screen
            };
            Action onBack = () =>
            {
                partyScreen.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };
            partyScreen.HandleUpdate(onSelected, onBack);
        }
        else if (state == GameState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };

            inventoryUI.HandleUpdate(onBack);
        } 
        else if (state == GameState.Shop)
        {
            ShopController.i.HandleUpdate();
        }
        else if (state == GameState.Instructions)
        {
            if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Escape)
                || Input.GetKeyDown(KeyCode.Backspace))
            {
                instructionsPanel.SetActive(false);
                state = GameState.FreeRoam;
            }
        } 
        
    }

    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }

    void OnMenuSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            //Animal
            partyScreen.gameObject.SetActive(true);
            state = GameState.PartyScreen;
        }
        else if (selectedItem == 1)
        {
            //Bag
            inventoryUI.gameObject.SetActive(true);
            state = GameState.Bag;
        }
        else if (selectedItem == 2)
        {
            //Save
            if(!PlayerPrefs.HasKey("SavedGame"))
                PlayerPrefs.SetString("SavedGame", "saveSlot1");
            SavingSystem.i.Save("saveSlot1");
            state = GameState.FreeRoam;
        }
        else if (selectedItem == 3)
        {
            //Load
            SavingSystem.i.Load("saveSlot1");
            state = GameState.FreeRoam;
        } 
        else if (selectedItem == 4)
        {
            //instructions
            state = GameState.Instructions;
            instructionsPanel.SetActive(true);
            
        } 
        else if (selectedItem == 5)
        {
            //save game
            if (!PlayerPrefs.HasKey("SavedGame"))
                PlayerPrefs.SetString("SavedGame", "saveSlot1");
            SavingSystem.i.Save("saveSlot1");
            //quit game
            Application.Quit();
        }

        
    }

    public IEnumerator MoveCamera(Vector2 moveOffset, bool waitForFadeOut=false)
    {
        //fade in
        yield return Fader.i.FadeIn(0.5f);

        worldCamera.transform.position += new Vector3 (moveOffset.x , moveOffset.y);

        if (waitForFadeOut)
            yield return Fader.i.FadeOut(0.5f);
        else
            StartCoroutine(Fader.i.FadeOut(0.5f));
    }

    public GameState State => state;
}
