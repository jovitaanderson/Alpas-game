using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizUI : MonoBehaviour
{
    [SerializeField] private QuizManager quizManager;
    [SerializeField] private Text questionText;
    [SerializeField] private Image questionImage;
    //[SerializeField] private List<Button> options;
    [SerializeField] private List<Image> options;
    [SerializeField] private Color correctCol, wrongCol, normalCol, selectedColor;

    private Question question;
    private Question prevQuestion;
    private bool answered;
    int currentAction;

    public bool? CorrectAns { get; private set; }

    private void Awake()
    {
        options[0].color = selectedColor;
    }
    private void Update()
    {
        HandleActionSelector();    
    }

    void HandleActionSelector()
    {
        if (!answered)
            options[currentAction].color = normalCol;

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

        if (!answered)
            options[currentAction].color = selectedColor;

        if (Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM")) || Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM1")))
        {
            if (!answered)
            {
                answered = true;
                bool val = quizManager.Answer(options[currentAction].GetComponentInChildren<Text>().text);

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

    public void SetQuestion(Question question)
    {
        if (this.question != null)
            prevQuestion = this.question;
        this.question = question;

        switch (question.questionType)
        {
            case QuestionType.TEXT:
                questionText.rectTransform.localPosition = new Vector3(-25f, 0, 0);
                questionText.rectTransform.sizeDelta = new Vector2(700f, questionText.preferredHeight);
                questionImage.transform.parent.gameObject.SetActive(false);
                break;
            case QuestionType.IMAGE:
                if (prevQuestion != null && prevQuestion.questionType == QuestionType.TEXT)
                {
                    questionText.rectTransform.localPosition = new Vector3(-125f, 0, 0);
                }
                questionText.rectTransform.sizeDelta = new Vector2(500f, questionText.preferredHeight);
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
