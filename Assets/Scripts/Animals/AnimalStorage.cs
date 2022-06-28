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

    [SerializeField] Image image;
    [SerializeField] GameObject lvlUpObj;
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;

    StorageMemberUI[] memberSlots;
    //maybe will create StorageMemberUI if got alot of difference

    int selection = 0;
    int maxAnimalsInStorage = 20;

    private void Awake()
    {
        foreach (var animal in animals) //for loop for every animal in animal list
        {
            animal.Init();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        memberSlots = GetComponentsInChildren<StorageMemberUI>(true);
        SetStorageData();
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

        UpdateMemberSelection(selection);
    }

    public void SwapAnimalStorage(Animal inParty, Animal inStorage)
    {
        var tempAnimal = inStorage;
        animals[selection] = inParty;
        partyScreen.SelectedMember = tempAnimal;
        SetStorageData();
        partyScreen.SetPartyData();
    }


    // Update is called once per frame
    void Update()
    {
        HandleUpdate(null, null);
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
                gameObject.SetActive(false);
                partyScreen.ResetSelection();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
            {
                gameObject.SetActive(false);
                partyScreen.ResetSelection();
            }
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < animals.Count; i++)
        {
            if (i == selectedMember)
            {
                image.sprite = animals[selectedMember].Base.FrontSprite;
                nameText.text = animals[selectedMember].Base.Name;
                levelText.text = "Lvl" + animals[selectedMember].Level;
                hpBar.SetHP((float)animals[selectedMember].HP / animals[selectedMember].MaxHp);
                memberSlots[i].SetSelected(true);
            }

            else
                memberSlots[i].SetSelected(false);
        }
    }

}

