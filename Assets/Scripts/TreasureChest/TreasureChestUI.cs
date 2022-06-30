using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureChestUI : MonoBehaviour
{
    [SerializeField] private TreasureChestManager treasureChestManager;
    [SerializeField] private Text questionText;
    [SerializeField] private Image questionImage;
    [SerializeField] private List<Image> options;
    [SerializeField] private Color correctCol, wrongCol, normalCol, selectedColor;

    private TreasureChestQuestion question;
    private bool answered;
    int currentAction;

    public static TreasureChestUI i { get; private set; }

    public bool? CorrectAns { get; private set; }

    private void Awake()
    {
        i = this;
        options[0].color = selectedColor;
        CorrectAns = null;
    }
    private void Update()
    {
        HandleActionSelector();
    }

    void HandleActionSelector()
    {
        if (!answered)
            options[currentAction].color = normalCol;

        if (Input.GetKeyDown(KeybindManager.i.keys["RIGHT"]) || Input.GetKeyDown(KeyCode.D))
            ++currentAction;
        else if (Input.GetKeyDown(KeybindManager.i.keys["LEFT"]) || Input.GetKeyDown(KeyCode.A))
            --currentAction;
        else if (Input.GetKeyDown(KeybindManager.i.keys["Down"]) || Input.GetKeyDown(KeyCode.S))
            currentAction += 2;
        else if (Input.GetKeyDown(KeybindManager.i.keys["UP"]) || Input.GetKeyDown(KeyCode.W))
            currentAction -= 2;

        //Restrict value of currentAction between 0 and 3
        currentAction = Mathf.Clamp(currentAction, 0, 3);

        if (!answered)
            options[currentAction].color = selectedColor;

        if (Input.GetKeyDown(KeybindManager.i.keys["CONFIRM"])) //return = enter key
        {
            if (!answered)
            {
                answered = true;
                bool val = treasureChestManager.Answer(options[currentAction].GetComponentInChildren<Text>().text);

                if (val)
                {
                    options[currentAction].color = correctCol;
                    CorrectAns = true;

                }
                else
                {
                    options[currentAction].color = wrongCol;
                    CorrectAns = false;
                }
            }
        }
    }

    public void Reset()
    {
        CorrectAns = null;
    }

    public void SetQuestion(TreasureChestQuestion question)
    {
        this.question = question;

        switch (question.questionType)
        {
            case TreasureChestQuestionType.TEXT:
                questionImage.transform.parent.gameObject.SetActive(false);
                break;
            case TreasureChestQuestionType.IMAGE:
                ImageHolder();
                questionImage.sprite = question.questionImg;
                break;
        }

        questionText.text = question.questionInfo;

        //shuffle the answer options
        List<string> answerList = ShuffleList.ShuffleListItems<string>(question.options);

        for (int i = 0; i < options.Count; i++)
        {
            options[i].GetComponentInChildren<Text>().text = answerList[i];
            options[i].name = answerList[i];
            options[i].color = normalCol;
        }

        answered = false;
    }

    void ImageHolder()
    {
        questionImage.transform.parent.gameObject.SetActive(true);
        questionImage.transform.gameObject.SetActive(true);
    }

   
}
