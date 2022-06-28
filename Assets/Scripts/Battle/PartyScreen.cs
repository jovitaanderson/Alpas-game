using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PartyScreenState { PartyScreen, ChoiceBox, Busy }

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;
    [SerializeField] GameObject choiceBox;


    PartyMemberUI[] memberSlots;
    List<Animal> animals;
    AnimalParty party;
    Text[] choices;

    PartyScreenState state;

    int selection = 0;
    int choiceSelection = 0;

    public Animal SelectedMember => animals[selection];

    //Party Screen can be called from different states like ActionSelection, RunningTurn, AboutToUse
    public BattleState? CalledFrom { get; set; }

    public void Init()
    {
        //Returns all the partyMemberUI components that are attached to the partyScreen
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);

        party = AnimalParty.GetPlayerParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    private void Start()
    {
        //get the children in choicebox
         choices = choiceBox.GetComponentsInChildren<Text>();
        UpdateChoiceBox(choiceSelection);
    }
    public void SetPartyData()
    {
        animals = party.Animals;

        for (int i = 0; i < memberSlots.Length; i++)
        {
        if (i < animals.Count)
        {
            memberSlots[i].gameObject.SetActive(true);
            memberSlots[i].Init(animals[i]);
        }
        else
            memberSlots[i].gameObject.SetActive(false);
        }

        UpdateMemberSelection(selection);

        messageText.text = "Choose an Animal";
    }

    //Handles party screen selection
    public void HandleUpdate(Action onSelected, Action onBack)
    {
        if (state == PartyScreenState.PartyScreen)
        {
            var prevSelection = selection;

            if (Input.GetKeyDown(KeyCode.RightArrow))
                ++selection;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                --selection;
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                selection += 2;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                selection -= 2;

            //Restrict value of currentMove between 0 and no. of animals moves
            selection = Mathf.Clamp(selection, 0, animals.Count - 1);

            if (selection != prevSelection)
                UpdateMemberSelection(selection);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                onSelected?.Invoke();
                if (onSelected == null)
                {
                    EnableChoiceBox(true);
                    messageText.text = $" {SelectedMember.Base.Name} was selected. Choose an Action";
                }

            }
            //Go back to select action screen if esc or backspace is pressed
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
            {
                onBack?.Invoke();

            }
        } 
        else if (state == PartyScreenState.ChoiceBox)
        {
            var prevChoiceSelection = choiceSelection;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++choiceSelection;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --choiceSelection;

            choiceSelection = Mathf.Clamp(choiceSelection, 0, 2);

            if (choiceSelection != prevChoiceSelection)
                UpdateChoiceBox(choiceSelection);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                //swap
                //delete 
                //add
                Debug.Log(choiceSelection);
            }
            //else, if press escape, then go back
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                EnableChoiceBox(false);
            }
        }
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < animals.Count; i++)
        {
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    //Set text on partyScreen
    public void SetMessageText(string message)
    {
        messageText.text = message;
    }


    public void EnableChoiceBox(bool enable)
    {
        choiceBox.SetActive(enable);
        if (enable)
        {
            state = PartyScreenState.ChoiceBox;
            messageText.text = $" {SelectedMember.Base.Name} was selected. Choose an Action";
        }
        else
        {
            state = PartyScreenState.PartyScreen;
            messageText.text = "Choose an Animal";
        }
    }
    void UpdateChoiceBox(int selectedChoice)
    {
        for (int i = 0; i < 3; i++)
        {
           if (i == selectedChoice)
                choices[i].color = GlobalSettings.i.HighlightedColor;
           else
                choices[i].color = Color.black;
        }
    }

}
