using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QuizManager : MonoBehaviour
{
    [SerializeField] QuizUI quizUI;
    [SerializeField] private QuizDataScriptable quizData;

    private List<Question> questions;

    private Question selectedQuestion;

    private void Awake()
    {
        questions = quizData.questions;
        SelectQuestion();
    }
    void SelectQuestion()
    {
        int val = Random.Range(0, questions.Count);
        selectedQuestion = questions[val];

        quizUI.SetQuestion(selectedQuestion);
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
public class Question
{
    public string questionInfo;
    public QuestionType questionType;
    public Sprite questionImg;
    public List<string> options;
    public string correctAns;
}

public enum QuestionType
{
    TEXT,
    IMAGE
}
