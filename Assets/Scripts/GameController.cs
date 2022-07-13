using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Menu, PartyScreen, Bag, Cutscene, Paused, Evolution, Shop, Instructions, Controls, TreasureChest, Question, AnimalList}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] public AnimalList animaList;

    [SerializeField] GameObject miniMapWindow;
    [SerializeField] GameObject walletUI;
    [SerializeField] GameObject instructionsPanel;


    GameState state;
    GameState prevState;
    GameState stateBeforeEvolution;


    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    MenuController menuController;
    SettingsManager keybindManager;

    public static GameController Instance {get; private set;}
    private void Awake()
    {
        Instance = this;

        menuController = GetComponent<MenuController>();
        keybindManager = GetComponent<SettingsManager>();

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
        //todo: remove comments when done debugging
        /*if (MainController.checkLoadGame == true)
        {
            string savedSlotName = PlayerPrefs.GetString("SavedGame");
            SavingSystem.i.Load(savedSlotName);
            state = GameState.FreeRoam; //todo: remove this line(?)
        }
        else 
        {
            PlayerPrefs.SetString("SavedGame", "saveSlot1");
            SavingSystem.i.Save("saveSlot1");
        // set seen animals to false
        }*/



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

        keybindManager.onBack += () =>
        {
            keybindManager.closeSettingsUI();
            state = GameState.FreeRoam;
        };

        EvolutionManager.i.OnStartEvolution += () =>
        {
            stateBeforeEvolution = state;
            state = GameState.Evolution;
        };
        EvolutionManager.i.OnCompleteEvolution += () =>
        {
            partyScreen.SetPartyData();
            state = stateBeforeEvolution;

            AudioManager.i.Play(CurrentScene.SceneMusic, fade: true); 
        };

        TreasureChestManager.i.OnStartTreasureChest += () => state = GameState.TreasureChest;
        TreasureChestManager.i.OnSelectTreasureChest += () => state = GameState.Question;
        TreasureChestManager.i.OnCompleteTreasureChest += () => state = GameState.FreeRoam;

        ShopController.i.OnStart += () => state = GameState.Shop;
        ShopController.i.OnFinish += () => state = GameState.FreeRoam;


        //todo: Remove once finish testing game
        //Auto set preadded animals in party/storage to seen since we manaually added them
        foreach (var animalInParty in playerController.GetComponent<AnimalParty>().Animals)
        {
            animaList.AnimalSeen(animalInParty);
        }
        foreach (var animalInStorage in playerController.GetComponent<AnimalStorage>().Animals)
        {
            animaList.AnimalSeen(animalInStorage);
        }

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

    public void StartBattle(BattleTrigger trigger)
    {   
        //We can get animalparty from plaayercontroller since they are both components of the player game object
        var playerParty = playerController.GetComponent<AnimalParty>();
        //FindObjectOfType<MapArea>() will get the reference and return the game object with MapArea
        var wildAnimal = CurrentScene.GetComponent<MapArea>().GetRandomWildAnimal(trigger);
        var wildAnimalCopy = new Animal(wildAnimal.Base, wildAnimal.Level);

        if (playerParty.GetHealthyAnimal() == null)
        {
            StartCoroutine(DialogManager.Instance.ShowDialogText("All your animals are fainted, cannot fight wild animals."));
        }
        else
        {
            //instructionsPanel.SetActive(false);
            keybindManager.closeSettingsUI();
            state = GameState.Battle;
            miniMapWindow.SetActive(false);
            walletUI.SetActive(false);
            battleSystem.gameObject.SetActive(true);
            worldCamera.gameObject.SetActive(false);

            battleSystem.StartBattle(playerParty, wildAnimalCopy, CurrentScene.Background, CurrentScene.BackgroundCircles, trigger);
        }
    }

    TrainerController trainer;
    public void StartTrainerBattle(TrainerController trainer)
    {
        keybindManager.closeSettingsUI();
        state = GameState.Battle;
        miniMapWindow.SetActive(false);
        walletUI.SetActive(false);
        walletUI.SetActive(false);
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        this.trainer = trainer;
        var playerParty = playerController.GetComponent<AnimalParty>();
        var trainerParty = trainer.GetComponent<AnimalParty>();
        //reset trainer party animals
        trainerParty.resetStatsAnimal();

        battleSystem.StartTrainerBattle(playerParty, trainerParty, CurrentScene.Background, CurrentScene.BackgroundCircles);
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
        AudioManager.i.Play(CurrentScene.SceneMusic, fade: true); 
        StartCoroutine(playerParty.RunEvolutions());
        //else
            
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();

            if (Input.GetKeyDown(SettingsManager.i.getKey("MENU")) || Input.GetKeyDown(SettingsManager.i.getKey("MENU1"))
                || Input.GetKeyDown(SettingsManager.i.getKey("BACK")) || Input.GetKeyDown(SettingsManager.i.getKey("BACK1")))
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
            /* Action onSelected = () =>
             {

                 //Todo: Go to Summary Screen
                 //option to swap or remove
                 partyScreen.EnableChoiceBox();
             };*/
            Action onBack = () =>
            {
                partyScreen.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };
            partyScreen.HandleUpdate(null, onBack);
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
        else if (state == GameState.TreasureChest)
        {
            TreasureChestManager.i.HandleUpdate();
        }
        else if (state == GameState.Question)
        {

        }
        else if (state == GameState.Instructions)
        {
            if (Input.GetKeyDown(SettingsManager.i.getKey("BACK")) || Input.GetKeyDown(SettingsManager.i.getKey("BACK1")))
            {
                state = GameState.FreeRoam;
            }
        }
        else if (state == GameState.Controls)
        {

        }
        else if (state == GameState.AnimalList)
        {
            Action onBack = () =>
            {
                animaList.animalListUI.SetActive(false);
                state = GameState.FreeRoam;
            };
            animaList.HandleUpdate(onBack);
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
            SavingSystem.i.Load(PlayerPrefs.GetString("SavedGame"));
            state = GameState.FreeRoam;
        } 
        else if (selectedItem == 4)
        {
            //animal List
            state = GameState.AnimalList;
            animaList.animalListUI.gameObject.SetActive(true);
            //AnimalListUI.HandleUpdate();
        }
        else if (selectedItem == 5)
        {
            //instructions
            //state = GameState.Instructions;
            //instructionsPanel.SetActive(true);
            //Controls
            state = GameState.Controls;
            keybindManager.openSettingsUI();
        } 
        else if (selectedItem == 6)
        {
            //save game
            if (!PlayerPrefs.HasKey("SavedGame"))
                PlayerPrefs.SetString("SavedGame", "saveSlot1");
            SavingSystem.i.Save("saveSlot1");
            //quit game
            Application.Quit();

            //TODO: quit game
            //Application.OpenURL("https://itch.io/");
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
