using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Script to handle selection difficulty and question of treasure chest
public class TreasureChestManager : MonoBehaviour
{
    [SerializeField] GameObject chestUI;
    [SerializeField] private TreasureChestDataScriptable easyChestData;
    [SerializeField] private TreasureChestDataScriptable mediumChestData;
    [SerializeField] private TreasureChestDataScriptable hardChestData;

    GameObject selectChest;
    //GameObject treasureChest;
    private TreasureChestUI treasureChestUI;

    ChestUI[] chestChildren;
    public int selectedItem = 0;
    public int treasureChest = 0;

    private List<TreasureChestQuestion> questions;
    private TreasureChestQuestion selectedQuestion;
    private TreasureChestQuestion prevQuestion;
    private float rewardedEasyMoney = 10f;
    private float rewardedMediumMoney = 50f;
    private float rewardedHardMoney = 100f;

    public event Action OnStartTreasureChest;
    public event Action OnSelectTreasureChest;
    public event Action OnCompleteTreasureChest;


    public static TreasureChestManager i { get; private set; }

    private void Awake()
    {
        i = this;
        selectChest = chestUI.transform.Find("TreasureChestSelectionUI").gameObject;
        chestChildren = selectChest.GetComponentsInChildren<ChestUI>();
        //treasureChest = chestUI.transform.Find("TreasureChestQuestionUI").gameObject;
        treasureChestUI = chestUI.transform.Find("TreasureChestQuestionUI").gameObject.GetComponent<TreasureChestUI>();
    }

    //Select chest type section
    public void OpenMenu()
    {
        selectedItem = 0;
        OnStartTreasureChest?.Invoke();
        selectChest.SetActive(true);
        UpdateChestSelection(selectedItem);
    }
    public void CloseMenu()
    {
        selectChest.SetActive(false);
    }

    public void HandleUpdate()
    {
        int prevSelection = selectedItem;

        if (Input.GetKeyDown(ControlManager.i.getKey("RIGHT")))
            ++selectedItem;
        else if (Input.GetKeyDown(ControlManager.i.getKey("LEFT")))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, chestChildren.Length - 1);

        if (prevSelection != selectedItem)
            UpdateChestSelection(selectedItem);

        //if press enter then go do action
        if (Input.GetKeyDown(ControlManager.i.getKey("CONFIRM")))
        {
            CloseMenu();
            OnSelectTreasureChest?.Invoke();
            StartCoroutine(onChestSelected(selectedItem));
        }
    }

    public void UpdateChestSelection(int selectedMember)
    {
        for (int i = 0; i < chestChildren.Length; i++)
        {
            if (i == selectedMember)
                chestChildren[i].SetSelected(true);
            else
                chestChildren[i].SetSelected(false);
        }
    }

    public IEnumerator onChestSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            //small chest
            treasureChest = 0;
            Debug.Log("Small chest");
            yield return TreasureChest();

        }
        else if (selectedItem == 1)
        {
            //Medium chest
            treasureChest = 1;
            Debug.Log("Medium chest");
            yield return TreasureChest();
            
        }
        else if (selectedItem == 2)
        {
            //Big chest
            treasureChest = 2;
            Debug.Log("Big chest");
            yield return TreasureChest();
            
        }
    }


    //Question for chest section
    public IEnumerator TreasureChest()
    {
        float rewardedMoney = 0f;
        switch (treasureChest)
        {
            case 0:
                rewardedMoney = rewardedEasyMoney;
                questions = easyChestData.questions;
                break;
            case 1:
                rewardedMoney = rewardedMediumMoney;
                questions = mediumChestData.questions;
                break;
            case 2:
                rewardedMoney = rewardedHardMoney;
                questions = hardChestData.questions;
                break;
            default:
                yield break;
        }
        
        treasureChestUI.gameObject.SetActive(true);

        SelectQuestion();

        AudioManager.i.PlaySfx(AudioId.OpenChest);

        treasureChestUI.Reset();
        yield return new WaitUntil(() => treasureChestUI.CorrectAns != null);

        //if qns answered correctly
        if (treasureChestUI.CorrectAns == true)
        {
            //Debug.Log("Qn is answered correctly");
            yield return new WaitForSeconds(0.5f);
            yield return DialogManager.Instance.ShowDialogText($"Good Job! You have answered correctly!");

            treasureChestUI.gameObject.SetActive(false);

            //reward coins into wallet
            Wallet.i.AddMoney(rewardedMoney);
            yield return DialogManager.Instance.ShowDialogText($"You have received {rewardedMoney} coins!");

        }
        else
        {
            //Debug.Log("Qn is answered wrongly");
            yield return new WaitForSeconds(0.5f);
            yield return DialogManager.Instance.ShowDialogText($"You got the answer wrong! Try again next time!");

            treasureChestUI.gameObject.SetActive(false);

        }

        OnCompleteTreasureChest?.Invoke();
    }

    void SelectQuestion()
    {
        if (selectedQuestion != null)
            prevQuestion = selectedQuestion;
        int val = UnityEngine.Random.Range(0, questions.Count);
        selectedQuestion = questions[val];

        while (prevQuestion == selectedQuestion)
        {
            int newVal = UnityEngine.Random.Range(0, questions.Count);
            selectedQuestion = questions[newVal];
        }


        treasureChestUI.SetQuestion(selectedQuestion);
    }

    public bool Answer(string answered)
    {
        bool correctAns = false;

        if (answered == selectedQuestion.correctAns)
        {
            //Correct
            correctAns = true;
        }
        else
        {
            //Todo: wrong, got 3 chances
        }

        //if 3 chances over or 1 answered correctly, then go back to evolution
        return correctAns;
    }
}

[System.Serializable]
public class TreasureChestQuestion
{
    public string questionInfo;
    public TreasureChestQuestionType questionType;
    public Sprite questionImg;
    public List<string> options;
    public string correctAns;
}

public enum TreasureChestQuestionType
{
    TEXT,
    IMAGE
}
