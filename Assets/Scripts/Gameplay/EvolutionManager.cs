using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] Image animalImage;

    public event Action OnStartEvolution;
    public event Action OnCompleteEvolution;

    public static EvolutionManager i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    //Evolve
    public IEnumerator Evolve(Animal animal, Evolution evolution)
    {
        OnStartEvolution?.Invoke();
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
        OnCompleteEvolution?.Invoke();

    }

}
