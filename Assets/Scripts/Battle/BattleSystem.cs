using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;


//to store battle state
public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, Bag, PartyScreen,AboutToUse, MoveToForget, BattleOver}
public enum BattleAction { Move, SwitchAnimal, UseItem, Run}
public enum BattleTrigger { LongGrass, Water }

//To control the entire battle
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject pokeballSprite;
    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] InventoryUI inventoryUI;

    [Header("Audio")]
    [SerializeField] string wildBattleMusic;
    [SerializeField] string trainerBattleMusic;
    [SerializeField] string battleVictoryMusic;

    [Header("background image")]
    [SerializeField] Image backgroundImage;
    [SerializeField] Sprite grassBackground;
    [SerializeField] Sprite waterBackground;
    [SerializeField] Sprite grassBackgroundCirclesBattle;
    [SerializeField] Sprite waterBackgroundCirclesBattle;
    [SerializeField] Image backgroundBattleCirclesImage1;
    [SerializeField] Image backgroundBattleCirclesImage2;

    public void GrassBackground(Sprite grassBackground)
    {
        this.grassBackground = grassBackground;
    }

    public void GrassCirclesBackground(Sprite grassBackgroundCirclesBattle)
    {
        this.grassBackgroundCirclesBattle = grassBackgroundCirclesBattle;
    }


    public event Action<bool> OnBattleOver;

    BattleState state;
    
    int currentAction;
    int currentMove;
    bool aboutToUseChoice = true;

    AnimalParty playerParty;
    AnimalParty trainerParty;
    Animal wildAnimal;

    bool isTrainerBattle = false;
    PlayerController player;
    TrainerController trainer;

    int escapeAttempts;
    MoveBase moveToLearn;

    private BattleTrigger battleTrigger;

    public void StartBattle(AnimalParty playerParty, Animal wildAnimal, BattleTrigger trigger = BattleTrigger.LongGrass)
    {
        this.playerParty = playerParty;
        this.wildAnimal = wildAnimal;
        GameObject.Find("GameController").GetComponent<GameController>().animalCharacterManager.characterDB.AnimalSeen(wildAnimal);
        player = playerParty.GetComponent<PlayerController>();
        isTrainerBattle = false;

        battleTrigger = trigger;

        AudioManager.i.Play(wildBattleMusic);

        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(AnimalParty playerParty, AnimalParty trainerParty, BattleTrigger trigger = BattleTrigger.LongGrass)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        battleTrigger = trigger;

        AudioManager.i.Play(trainerBattleMusic);

        StartCoroutine(SetupBattle());
    }


    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();

        backgroundImage.sprite = (battleTrigger == BattleTrigger.LongGrass) ? grassBackground : waterBackground;
        backgroundBattleCirclesImage1.sprite = (battleTrigger == BattleTrigger.LongGrass) ? 
            grassBackgroundCirclesBattle : waterBackgroundCirclesBattle;
        backgroundBattleCirclesImage2.sprite = (battleTrigger == BattleTrigger.LongGrass) ? 
            grassBackgroundCirclesBattle : waterBackgroundCirclesBattle;

        if (!isTrainerBattle)
        {
            //wild pokemon battle
            playerUnit.Setup(playerParty.GetHealthyAnimal());
            enemyUnit.Setup(wildAnimal);

            dialogBox.SetMoveNames(playerUnit.Animal.Moves);
            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Animal.Base.Name} appeared.");
        } 
        else
        {
            //trainer battle

            //show trainer and player sprites
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;
            yield return dialogBox.TypeDialog($"{trainer.Name} wants to battle");

            //send out first pokemon of the trainer

            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyAnimal = trainerParty.GetHealthyAnimal();
            GameObject.Find("GameController").GetComponent<GameController>().animalCharacterManager.characterDB.AnimalSeen(enemyAnimal);
            enemyUnit.Setup(enemyAnimal);
            yield return dialogBox.TypeDialog($"{trainer.Name} send out {enemyAnimal.Base.Name}");

            // Send out first pokemon of the player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerAnimal = playerParty.GetHealthyAnimal();
            playerUnit.Setup(playerAnimal);
            yield return dialogBox.TypeDialog($"Go {playerAnimal.Base.Name} !");
            dialogBox.SetMoveNames(playerUnit.Animal.Moves);
        }

        partyScreen.Init();
        escapeAttempts = 0;
        ActionSelection();
    }

    /*void ChooseFirstTurn()
    {
        if (playerUnit.Animal.Speed >= enemyUnit.Animal.Speed)
            ActionSelection();
        else
            StartCoroutine(EnemyMove());
    }  */

    void BattleOver(bool won) //state
    {
        state = BattleState.BattleOver;
        playerParty.Animals.ForEach(p => p.OnBattleOver());
        playerUnit.Hud.ClearData();
        enemyUnit.Hud.ClearData();
        OnBattleOver(won); //event
    }

    //Player chooses to fight or run
    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);
    }

    void OpenBag()
    {
        state = BattleState.Bag;
        inventoryUI.gameObject.SetActive(true);

    }

    void OpenPartyScreen()
    {
        partyScreen.CalledFrom = state;
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator AboutToUse(Animal newAnimal)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{trainer.Name} is about to use {newAnimal.Base.Name}. Do you want to change a new Animal?");

        state = BattleState.AboutToUse;
        dialogBox.EnableChoiceBox(true);
    }

    IEnumerator ChooseMoveToForget(Animal animal, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"Choose a move you want to forget");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(animal.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = BattleState.MoveToForget;
    }
    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Animal.CurrentMove = playerUnit.Animal.Moves[currentMove];
            enemyUnit.Animal.CurrentMove = enemyUnit.Animal.GetRandomMove();

            int playerMovePriority = playerUnit.Animal.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Animal.CurrentMove.Base.Priority;

            // Check who goes first
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if (enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit.Animal.Speed >= enemyUnit.Animal.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst)? enemyUnit: playerUnit;

            var secondAnimal = secondUnit.Animal;

            // First Turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Animal.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondAnimal.HP > 0)
            {
                // Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Animal.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchAnimal)
            {   
                var selectedAnimal = partyScreen.SelectedMember;
                state = BattleState.Busy;
                yield return SwitchAnimal(selectedAnimal);
            }

            else if (playerAction == BattleAction.UseItem) 
            {
                //this is handled from item screen, so do nothing and skip to enemy move
                dialogBox.EnableActionSelector(false);
                //yield return ThrowPokeball();
            }
            else if (playerAction == BattleAction.Run) 
            {
                //TODO: edge case (?) if we run shld all status be gone? e.g. if my animal is sleep and run away
                yield return TryToEscape();
            }

            // Enemy Turn
            var enemyMove = enemyUnit.Animal.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }

        if (state != BattleState.BattleOver)
            ActionSelection();
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Animal.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Animal);
            yield return sourceUnit.Hud.WaitForHPUpdate();
            yield break;
        }


        yield return ShowStatusChanges(sourceUnit.Animal);

        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Animal.Base.Name} used {move.Base.Name}");

        if (CheckIfMoveHits(move, sourceUnit.Animal, targetUnit.Animal))
        { 

            sourceUnit.PlayAttackAnimation();
            AudioManager.i.PlaySfx(move.Base.Sound);

            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();

            AudioManager.i.PlaySfx(AudioId.Hit);

            if (move.Base.Category == MoveCategory.Status) //Status move doesnt do damage
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Animal, targetUnit.Animal, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Animal.TakeDamage(move, sourceUnit.Animal);
                yield return targetUnit.Hud.WaitForHPUpdate();
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Animal.HP > 0)
            {
                foreach (var secondary in move.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit.Animal, targetUnit.Animal, secondary.Target);
                }
            }

            if (targetUnit.Animal.HP <= 0)
            {
                yield return HandleAnimalFainted(targetUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Animal.Base.Name}'s attack missed");

        }
    }
    IEnumerator RunMoveEffects(MoveEffects effects, Animal source, Animal target, MoveTarget moveTarget)
    {
        //Stat Boosting
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }

        //Status Condition
        if(effects.Status != ConditionID.none)
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.SetStatus(effects.Status);
            }
            else
            {
                target.SetStatus(effects.Status);
            }
        }

        //Volatile Status Condition
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }


        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        //It pauses the RunAfterTurn until the state is RunningTurn then it will continue to excuted the following lines
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        //Statuses like burn or psn will hurt the pokemon after the turn
        sourceUnit.Animal.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Animal);
        yield return sourceUnit.Hud.WaitForHPUpdate();
        if (sourceUnit.Animal.HP <= 0)
        {
            yield return HandleAnimalFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);

        }
    }

    bool CheckIfMoveHits(Move move, Animal source, Animal target)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;
        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };
        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];
        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;

    }

    IEnumerator ShowStatusChanges(Animal animal)
    {
        while (animal.StatusChanges.Count > 0)
        {
            var message = animal.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator HandleAnimalFainted(BattleUnit faintedUnit) 
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Animal.Base.Name} Fainted");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayerUnit) //is an enemyunit
        {
            bool battleWon = true;
            if (isTrainerBattle)
                battleWon = trainerParty.GetHealthyAnimal() == null;

            if (battleWon)
                AudioManager.i.Play(battleVictoryMusic);

            //exp gain
            int expYield = faintedUnit.Animal.Base.ExpYield;
            int enemyLevel = faintedUnit.Animal.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;
            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7); 
            playerUnit.Animal.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Animal.Base.Name} gained {expGain} exp");
            yield return playerUnit.Hud.SetExpSmooth();

            //ToDo: might implement the questions here so that player can answer questions before gaining exp
            
            //check level up
            while (playerUnit.Animal.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Animal.Base.Name} grew to level {playerUnit.Animal.Level}");

                //try to learn a new move
                var newMove = playerUnit.Animal.GetLearnableMoveAtCurrLevel();
                if (newMove != null)
                {
                    if (playerUnit.Animal.Moves.Count < AnimalBase.MaxNumOfMoves)
                    {
                        playerUnit.Animal.LearnMove(newMove);
                        yield return dialogBox.TypeDialog($"{playerUnit.Animal.Base.Name} learned {newMove.Base.Name}");
                        dialogBox.SetMoveNames(playerUnit.Animal.Moves);
                    } 
                    else 
                    {
                        yield return dialogBox.TypeDialog($"{playerUnit.Animal.Base.Name} is trying to learn {newMove.Base.Name},");
                        yield return dialogBox.TypeDialog($"but it cannot learn more than {AnimalBase.MaxNumOfMoves} moves.");
                        yield return ChooseMoveToForget(playerUnit.Animal, newMove.Base);
                        yield return new WaitUntil(() => state != BattleState.MoveToForget);
                        yield return new WaitForSeconds(2f);

                    }
                }

                yield return playerUnit.Hud.SetExpSmooth(true);
            }

            yield return new WaitForSeconds(1f);
        }

        CheckForBattleOver(faintedUnit);
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            //Check if any other healthy animal before ending the battle
            var nextPokemon = playerParty.GetHealthyAnimal();
            if (nextPokemon != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
        {
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextAnimal = trainerParty.GetHealthyAnimal();
                if (nextAnimal != null)
                    StartCoroutine(AboutToUse(nextAnimal));
                else 
                    BattleOver(true);
            }

        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!");
        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("It's not very effective!");
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        } 
        else if (state == BattleState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleState.ActionSelection;
            };

            Action<ItemBase> onItemUsed = (ItemBase usedItem) =>
            {
                StartCoroutine(OnItemUsed(usedItem));
            };
            inventoryUI.HandleUpdate(onBack, onItemUsed);
        }
        else if (state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
        else if (state == BattleState.MoveToForget)
        {
            Action<int> onMoveSelected = (moveIndex) => {
                moveSelectionUI.gameObject.SetActive(false);
                if (moveIndex == AnimalBase.MaxNumOfMoves) 
                {
                    //Dont learn new move
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Animal.Base.Name} did not learn {moveToLearn.Name}"));
                }
                else 
                {
                    //forget selected move and learn the new move
                    var selectedMove = playerUnit.Animal.Moves[moveIndex].Base;
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Animal.Base.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}"));
                    playerUnit.Animal.Moves[moveIndex] = new Move(moveToLearn);
                }
                moveToLearn = null;
                state = BattleState.RunningTurn;
            };
            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }

        //if (Input.GetKeyDown(KeyCode.T))
        //    StartCoroutine(ThrowPokeball());
    }
    void HandleActionSelection()
    {
        if (Input.GetKeyDown(SettingsManager.i.getKey("RIGHT")) || Input.GetKeyDown(SettingsManager.i.getKey("RIGHT1")))
            ++currentAction;
        else if (Input.GetKeyDown(SettingsManager.i.getKey("LEFT")) || Input.GetKeyDown(SettingsManager.i.getKey("LEFT1")))
            --currentAction;
        else if (Input.GetKeyDown(SettingsManager.i.getKey("DOWN")) || Input.GetKeyDown(SettingsManager.i.getKey("DOWN1")))
            currentAction += 2;
        else if (Input.GetKeyDown(SettingsManager.i.getKey("UP")) || Input.GetKeyDown(SettingsManager.i.getKey("UP1")))
            currentAction -= 2;

        //Restrict value of currentAction between 0 and 3
        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM")) || Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM1")))
        {
            if (currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Bag
                //StartCoroutine(RunTurns(BattleAction.UseItem));
                OpenBag();
            }
            else if (currentAction == 2)
            {
                // Animal
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                // Run
                //OnBattleOver(false);
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(SettingsManager.i.getKey("RIGHT")) || Input.GetKeyDown(SettingsManager.i.getKey("RIGHT1")))
            ++currentMove;
        else if (Input.GetKeyDown(SettingsManager.i.getKey("LEFT")) || Input.GetKeyDown(SettingsManager.i.getKey("LEFT1")))
            --currentMove;
        else if (Input.GetKeyDown(SettingsManager.i.getKey("DOWN")) || Input.GetKeyDown(SettingsManager.i.getKey("DOWN1")))
            currentMove += 2;
        else if (Input.GetKeyDown(SettingsManager.i.getKey("UP")) || Input.GetKeyDown(SettingsManager.i.getKey("UP1")))
            currentMove -= 2;

        //Restrict value of currentMove between 0 and no. of animals moves
        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Animal.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Animal.Moves[currentMove]);

        if (Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM")) || Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM1")))
        {
            var move = playerUnit.Animal.Moves[currentMove];
            if (move.PP == 0) return;

            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }

        //When esc key/backspace key press he goes back to selection screen
        else if (Input.GetKeyDown(SettingsManager.i.getKey("BACK")) || Input.GetKeyDown(SettingsManager.i.getKey("BACK1")))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }

    }

    //Handles party screen selection
    void HandlePartySelection()
    {
        Action onSelected = () =>
        {
            var selectedMember = partyScreen.SelectedMember;
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted pokemon");
                return;
            }
            if (selectedMember == playerUnit.Animal)
            {
                partyScreen.SetMessageText("You can't switch with the same pokemon");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (partyScreen.CalledFrom == BattleState.ActionSelection)
            {
                StartCoroutine(RunTurns(BattleAction.SwitchAnimal));
            }
            else
            {
                state = BattleState.Busy;
                bool isTrainerAboutToUse = partyScreen.CalledFrom == BattleState.AboutToUse;
                StartCoroutine(SwitchAnimal(selectedMember, isTrainerAboutToUse));
            }

            partyScreen.CalledFrom = null;
        };

        Action onBack = () =>
        {
            if (playerUnit.Animal.HP <= 0)
            {
                partyScreen.SetMessageText("You have to choose a pokemon to continue");
                return;
            }
            partyScreen.gameObject.SetActive(false);

            if (partyScreen.CalledFrom == BattleState.AboutToUse)
            {
                StartCoroutine(SendNextTrainerAnimal());
            }
            else
                ActionSelection();

            partyScreen.CalledFrom = null;
        };
        
        partyScreen.HandleUpdate(onSelected, onBack);


        
    }

    void HandleAboutToUse()
    {
        if(Input.GetKeyDown(SettingsManager.i.getKey("UP")) || Input.GetKeyDown(SettingsManager.i.getKey("UP1")) 
            || Input.GetKeyDown(SettingsManager.i.getKey("DOWN")) || Input.GetKeyDown(SettingsManager.i.getKey("DOWN1")))
            aboutToUseChoice = !aboutToUseChoice; //since there are only two options yes/no

        dialogBox.UpdateChoiceBox(aboutToUseChoice);

        if (Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM")) || Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM1")))
        {
            dialogBox.EnableChoiceBox(false);
            if(aboutToUseChoice == true)
            {
                //yes Option
                OpenPartyScreen();
            }
            else
            {
                //no option
                StartCoroutine(SendNextTrainerAnimal());
            }
        }
        else if (Input.GetKeyDown(SettingsManager.i.getKey("BACK")) || Input.GetKeyDown(SettingsManager.i.getKey("BACK1")))
        {
            dialogBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTrainerAnimal());
        }
        
    }

    //Switch pokemon method
    IEnumerator SwitchAnimal(Animal newAnimal, bool isTrainerAboutToUse = false)
    {
        if (playerUnit.Animal.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Animal.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newAnimal);
        dialogBox.SetMoveNames(newAnimal.Moves);
        yield return dialogBox.TypeDialog($"Go {newAnimal.Base.Name} !");

        if (isTrainerAboutToUse)
            StartCoroutine(SendNextTrainerAnimal());
        else
            state = BattleState.RunningTurn;
        
    }

    IEnumerator SendNextTrainerAnimal()
    {
        state = BattleState.Busy;

        var nextAnimal = trainerParty.GetHealthyAnimal();
        enemyUnit.Setup(nextAnimal);
        yield return dialogBox.TypeDialog($"{trainer.Name} send out {nextAnimal.Base.Name}!");

        state = BattleState.RunningTurn;
    }

    IEnumerator OnItemUsed(ItemBase usedItem)
    {
        state = BattleState.Busy;
        inventoryUI.gameObject.SetActive(false);

        if (usedItem is AnimalCaptureItem)
        {
            yield return ThrowPokeball((AnimalCaptureItem)(usedItem));
        }

        StartCoroutine(RunTurns(BattleAction.UseItem));
    }

    IEnumerator ThrowPokeball(AnimalCaptureItem animalCaptureItem) {

        state = BattleState.Busy;
        
        if (isTrainerBattle) {
            yield return dialogBox.TypeDialog($"You cannot steal the trainers' animal!");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.UserName} used {animalCaptureItem.Name.ToUpper()}"); //player.Name

        var pokeballObj = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2,0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
        pokeball.sprite = animalCaptureItem.Icon;

        //Animations
        yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0,1), 2f, 1, 1.5f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 2f, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchPokemon(enemyUnit.Animal, animalCaptureItem);
        //shake 3 times:
        for (int i = 0 ; i < Mathf.Min(shakeCount, 3); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0,0,10f), 0.8f).WaitForCompletion();
        }
        if (shakeCount == 4) {
            //Pokemon is caught
            yield return dialogBox.TypeDialog($"{enemyUnit.Animal.Base.Name} was caught");
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();
            if (playerParty.Animals.Count < 6)
            {
                yield return dialogBox.TypeDialog($"{enemyUnit.Animal.Base.Name} has been added to your party");
            } else if (playerParty.animalStorage.Animals.Count < playerParty.animalStorage.maxAnimalsInStorage)
            {
                yield return dialogBox.TypeDialog($"{enemyUnit.Animal.Base.Name} has been added to your storage");
            } else
            {
                yield return dialogBox.TypeDialog($"Party and storage is full! {enemyUnit.Animal.Base.Name} cannot be caught");
            }
            playerParty.AddAnimal(enemyUnit.Animal);

            Destroy(pokeball);
            BattleOver(true);
        } else {
            //pokemon broke out
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            if (shakeCount < 2) 
            {
                yield return dialogBox.TypeDialog($"{enemyUnit.Animal.Base.Name} broke free");
            } else {
                yield return dialogBox.TypeDialog($"Almost caught it");
            }

            Destroy(pokeball);
            state = BattleState.RunningTurn;
        }
    }

    int TryToCatchPokemon(Animal animal, AnimalCaptureItem animalCaptureItem)
    {
        float a = (3 * animal.MaxHp - 2 * animal.HP) * animal.Base.CatchRate * animalCaptureItem.CatchRateModifier * ConditionsDB.GetStatusBonus(animal.Status) / (3 * animal.MaxHp);

        if (a >= 255) {
            return 4; //return number of shake count
        }

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4) 
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;

            ++shakeCount;
        }
        return shakeCount;
    }

    IEnumerator TryToEscape() 
    {
        state = BattleState.Busy;

        if (isTrainerBattle) 
        {
            yield return dialogBox.TypeDialog($"You cannot run from trainer battles!");
            state = BattleState.RunningTurn;
            yield break;
        }

        ++escapeAttempts;

        int playerSpeed = playerUnit.Animal.Speed;
        int enemySpeed = enemyUnit.Animal.Speed;

        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"Ran away safely");
            BattleOver(true);
        }
        else 
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0,256) < f)
            {
                yield return dialogBox.TypeDialog($"Ran away safely!");
                BattleOver(true);
            }
            else 
            {
                yield return dialogBox.TypeDialog($"Failed to escape!");
                state = BattleState.RunningTurn;
            }
        }
    }
}
