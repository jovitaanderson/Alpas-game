using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] Image animalImage;
    [SerializeField] GameObject quizUI;

    private QuizUI quizScript;

    public event Action OnStartEvolution;
    public event Action OnCompleteEvolution;

    public static EvolutionManager i { get; private set; }

    private void Awake()
    {
        i = this;
        quizScript = quizUI.GetComponent<QuizUI>();
    }

    //Evolve
    public IEnumerator Evolve(Animal animal, Evolution evolution)
    {
        OnStartEvolution?.Invoke();
        quizUI.SetActive(true);
        //yield return new WaitForSeconds(2f);
        
        //if qns answered correctly
        //Todo: fix this null excerption problem
        if (quizScript?.CorrectAns == true)
        {
            yield return DialogManager.Instance.ShowDialogText($"Good Job! You have answered correctly!");
            quizUI.SetActive(false);
            
            evolutionUI.SetActive(true);

            //pokemon before evoluion
            animalImage.sprite = animal.Base.FrontSprite;
            yield return DialogManager.Instance.ShowDialogText($"{animal.Base.Name} is evolving");
            var oldAnimal = animal.Base;

            animal.Evolve(evolution);

            //evolved pokemon
            animalImage.sprite = animal.Base.FrontSprite;
            yield return DialogManager.Instance.ShowDialogText($"{oldAnimal.Name} evolved into {animal.Base.Name}");

            //deactive ui and continue gameplay
            evolutionUI.SetActive(false);
        }
       
        
        OnCompleteEvolution?.Invoke();

    }

}
