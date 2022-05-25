using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//to store battle state
public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver}

//To control the entire battle
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    int currentMember;

    AnimalParty playerParty;
    Animal wildAnimal;

    public void StartBattle(AnimalParty playerParty, Animal  wildAnimal)
    {
        this.playerParty = playerParty;
        this.wildAnimal = wildAnimal;
        StartCoroutine(SetupBattle());
    }
    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildAnimal);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Animal.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Animal.Base.Name} appeared.");

        ChooseFirstTurn();
    }
    void ChooseFirstTurn()
    {
        if (playerUnit.Animal.Speed >= enemyUnit.Animal.Speed)
            ActionSelection();
        else
            StartCoroutine(EnemyMove());
    }

    void BattleOver(bool won) //state
    {
        state = BattleState.BattleOver;
        playerParty.Animals.ForEach(p => p.OnBattleOver());
        OnBattleOver(won); //event
    }

    //Player chooses to fight or run
    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);
    }
    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Animals);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    //Player animal will perform move, and enemy will take damage
    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;

        var move = playerUnit.Animal.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);

        //If the battle stat was not changed by RunMove, then go to next step
        if(state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
        
    }

    IEnumerator EnemyMove() {

        state = BattleState.PerformMove;

        var move = enemyUnit.Animal.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        //If the battle stat was not changed by RunMove, then go to next step
        if (state == BattleState.PerformMove)
            ActionSelection();
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Animal.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Animal);
            yield return sourceUnit.Hud.UpdateHP();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Animal);

        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Animal.Base.Name} used {move.Base.Name}");

        if (CheckIfMoveHits(move, sourceUnit.Animal, targetUnit.Animal))
        { 

            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status) //Status move doesnt do damage
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Animal, targetUnit.Animal, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Animal.TakeDamage(move, sourceUnit.Animal);
                yield return targetUnit.Hud.UpdateHP();
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
                yield return dialogBox.TypeDialog($"{targetUnit.Animal.Base.Name} Fainted");
                targetUnit.PlayFaintAnimation();
                yield return new WaitForSeconds(2f);

                CheckForBattleOver(targetUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Animal.Base.Name}'s attack missed");

        }

        //Statuses like burn or psn will hurt the pokemon after the turn
        sourceUnit.Animal.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Animal);
        yield return sourceUnit.Hud.UpdateHP();
        if (sourceUnit.Animal.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Animal.Base.Name} Fainted");
            sourceUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
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
            target.SetStatus(effects.Status);
        }

        //Volatile Status Condition
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }


        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
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

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            //Check if any other healthy animal before ending the battle
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
            BattleOver(true);
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
    }
    void HandleActionSelection()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        //Restrict value of currentAction between 0 and 3
        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Return)) //return = enter key
        {
            if (currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Bag
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
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        //Restrict value of currentMove between 0 and no. of animals moves
        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Animal.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Animal.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove());
        }

        //When esc key/backspace key press he goes back to selection screen
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }

    }

    //Handles party screen selection
    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;

        //Restrict value of currentMove between 0 and no. of animals moves
        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Animals.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            var selectedMember = playerParty.Animals[currentMember];
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
            state = BattleState.Busy;
            StartCoroutine(SwitchAnimal(selectedMember));
        }
        //Go back to select action screen if esc or backspace is pressed
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    //Switch pokemon method
    IEnumerator SwitchAnimal(Animal newAnimal)
    {
        bool currentAnimalFainted = true;
        if (playerUnit.Animal.HP > 0)
        {
            currentAnimalFainted = false;
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Animal.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newAnimal);
        dialogBox.SetMoveNames(newAnimal.Moves);
        yield return dialogBox.TypeDialog($"Go {newAnimal.Base.Name} !");

        if (currentAnimalFainted)
            ChooseFirstTurn();
        else
            StartCoroutine(EnemyMove());
    }

}
