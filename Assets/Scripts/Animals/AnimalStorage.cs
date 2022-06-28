using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalStorage : MonoBehaviour
{
    [SerializeField] List<Animal> animals;
    [SerializeField] Text messageText;
    [SerializeField] PartyScreen partyScreen;
    PartyMemberUI[] memberSlots;

    int selection = 0;
    int maxAnimalsInStorage = 20;


    // Start is called before the first frame update
    void Start()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);

    }

    public void SetStorageData()
    {
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
    }

    public void AddAnimalStorage(Animal newAnimal)
    {
        if (animals.Count < maxAnimalsInStorage)
        {
            animals.Add(newAnimal);
            //OnUpdated?.Invoke();
        }
        else
        {
            messageText.text = "The storage is full, cannot add more animals.";
            //TODO: Add to the PC once thats implenmented
        }
    }

    public void RemoveAnimalStorage(Animal newAnimal)
    {
        animals.Remove(newAnimal);
    }

    public void SwapAnimalStorage(Animal inParty, Animal inStorage)
    {

    }


    // Update is called once per frame
    void Update()
    {

    }

    public void HandleUpdate(Action onSelected, Action onBack)
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

           selection = Mathf.Clamp(selection, 0, animals.Count - 1);

            if (selection != prevSelection)
                UpdateMemberSelection(selection);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                SwapAnimalStorage(partyScreen.SelectedMember, animals[selection]);
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

}

