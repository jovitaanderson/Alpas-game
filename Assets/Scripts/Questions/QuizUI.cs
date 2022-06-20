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

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            currentAction -= 2;

        //Restrict value of currentAction between 0 and 3
        currentAction = Mathf.Clamp(currentAction, 0, 3);

        if (!answered)
            options[currentAction].color = selectedColor;

        if (Input.GetKeyDown(KeyCode.Return)) //return = enter key
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

    //options[currentAction].color = normalCol;
    //        ++currentAction;
    //        options[currentAction].color = selectedColor;

    public void Reset()
    {
        CorrectAns = null;
    }

    public void SetQuestion(Question question)
    {
        this.question = question;

        switch (question.questionType)
        {
            case QuestionType.TEXT:
                questionImage.transform.parent.gameObject.SetActive(false);
                break;
            case QuestionType.IMAGE:
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

    private void OnClick(Button btn)
    {
        if (!answered)
        {
            answered = true;
            bool val = quizManager.Answer(options[currentAction].GetComponentInChildren<Text>().text);

            if (val)
            {
                btn.image.color = correctCol;
                CorrectAns = true;

            } else
            {
                btn.image.color = wrongCol;
                CorrectAns = false;
            }
        }
    }

}
