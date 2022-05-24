using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;

    PartyMemberUI[] memberSlots;
    List<Animal> animals;

    public void Init()
    {
        //Returns all the partyMemberUI components that are attached to the partyScreen
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }
    public void SetPartyData(List<Animal> animals)
    {
        this.animals = animals;

        for (int i = 0; i < memberSlots.Length; i++)
        {
        if (i < animals.Count)
            memberSlots[i].SetData(animals[i]);
        else
            memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Pokemon";
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
}
