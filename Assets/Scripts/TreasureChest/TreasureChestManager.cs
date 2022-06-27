using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureChestManager : MonoBehaviour
{
    [SerializeField] GameObject selectChest;


    ChestUI[] chestChildren;

    public int selectedItem = 0;


    [SerializeField] private TreasureChestDataScriptable treasureChestData;
    private List<TreasureChestQuestion> questions;
    private TreasureChestQuestion selectedQuestion;

    [SerializeField] GameObject treasureChest;
    [SerializeField] TreasureChestUI treasureChestUI;

    public event Action OnStartTreasureChest;
    public event Action OnCompleteTreasureChest;

    private TreasureChestUI treasureChestScript;

    private float rewardedMoney = 10f;

    public static TreasureChestManager i { get; private set; }

    private void Awake()
    {
        i = this;
        treasureChestScript = treasureChest.GetComponent<TreasureChestUI>();
        chestChildren = selectChest.GetComponentsInChildren<ChestUI>();
        Debug.Log(chestChildren.Length);

    }

    //Select Chest type code

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

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, chestChildren.Length - 1);

        if (prevSelection != selectedItem)
            UpdateChestSelection(selectedItem);

        //if press enter then go do action
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log(selectedItem);
            onChestSelected(selectedItem);
            CloseMenu();
            //open question

        }
        //else, if press escape, then go back
        /*else if (Input.GetKeyDown(KeyCode.Escape))
        {
            onBack?.Invoke();
            CloseMenu();
        }*/
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
            Debug.Log("Small chest");
            yield return TreasureChest();

        }
        else if (selectedItem == 1)
        {
            //Medium chest
            Debug.Log("Medium chest");
        }
        else if (selectedItem == 2)
        {
            //Big chest
            Debug.Log("Big chest");
        }
    }


    //Question related code

    public IEnumerator TreasureChest()
    {
        questions = treasureChestData.questions;
        treasureChest.SetActive(true);

        SelectQuestion();

        //AudioManager.i.PlayMusic(evolutionMusic);

        treasureChestScript.Reset();
        yield return new WaitUntil(() => treasureChestScript.CorrectAns != null);

        //if qns answered correctly
        if (treasureChestScript.CorrectAns == true)
        {
            //Debug.Log("Qn is answered correctly");
            yield return new WaitForSeconds(0.5f);
            yield return DialogManager.Instance.ShowDialogText($"Good Job! You have answered correctly!");

            treasureChest.SetActive(false);

            //reward coins into wallet
            Wallet.i.AddMoney(rewardedMoney);
            yield return DialogManager.Instance.ShowDialogText($"You have received {rewardedMoney} coins!");

        }
        else
        {
            //Debug.Log("Qn is answered wrongly");
            yield return new WaitForSeconds(0.5f);
            yield return DialogManager.Instance.ShowDialogText($"You got the answer wrong! Try again next time!");

            treasureChest.SetActive(false);

        }

        OnCompleteTreasureChest?.Invoke();
    }

    void SelectQuestion()
    {
        int val = UnityEngine.Random.Range(0, questions.Count);
        selectedQuestion = questions[val];

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
