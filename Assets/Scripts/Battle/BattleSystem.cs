using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//to store battle state
public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy}

//To control the entire battle
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;

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
        playerHud.setData(playerUnit.Animal);
        enemyHud.setData(enemyUnit.Animal);

        dialogBox.SetMoveNames(playerUnit.Animal.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Animal.Base.Name} appeared.");

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    //Player animal will perform move, and enemy will take damage
    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerUnit.Animal.Moves[currentMove];
        move.PP--;
        yield return dialogBox.TypeDialog($"{playerUnit.Animal.Base.Name} used {move.Base.Name}");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();

        var damageDetails = enemyUnit.Animal.TakeDamage(move, playerUnit.Animal);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Animal.Base.Name} Fainted");
            enemyUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver(true); 
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove() {
        state = BattleState.Busy;

        var move = enemyUnit.Animal.Moves[currentMove];
        move.PP--;
        yield return dialogBox.TypeDialog($"{enemyUnit.Animal.Base.Name} used {move.Base.Name}");

        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

       playerUnit.PlayHitAnimation();

        var damageDetails = playerUnit.Animal.TakeDamage(move, playerUnit.Animal);
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Animal.Base.Name} Fainted");
            playerUnit.PlayFaintAnimation();
            
            yield return new WaitForSeconds(2f);

            //Check if any other healthy animal before ending the battle

            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                playerUnit.Setup(nextPokemon);
                playerHud.setData(nextPokemon);

                dialogBox.SetMoveNames(nextPokemon.Moves);

                yield return dialogBox.TypeDialog($"Go {nextPokemon.Base.Name}");

                PlayerAction();
            }
            else
            {
                OnBattleOver(false);
            }
        }
        else
        {
            PlayerAction();
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
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }
    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Return)) //return = enter key
        {
            if (currentAction == 0)
            {
                // FightI
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                // Run
                //OnBattleOver(false);
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Animal.Moves.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Animal.Moves.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Animal.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
            
        }

    }
}
