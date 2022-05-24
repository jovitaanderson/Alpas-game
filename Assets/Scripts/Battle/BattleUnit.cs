 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

//A script for our player unit ( animal)  in battle 
public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }

    public BattleHud Hud
    {
        get { return hud; }
    }
             
    public Animal Animal { get; set; }

    Image image;
    Vector3 originalPos;
    Color originalColor;
    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup(Animal animal)
    {
        Animal = animal;
        if (isPlayerUnit) image.sprite = Animal.Base.BackSprite;
        else image.sprite = Animal.Base.FrontSprite;

        hud.SetData(animal);

        image.color = originalColor;
        PlayEnterAnimation();
    }
    
    //To allow animal image to slide in frame
    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, originalPos.y);

        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    //Move animal image to show that it is "attacking"
    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));

    }

    //Darken image to show it is being "hit"
    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }


    //we use join instead of append because we want it to fall and go invisible at the same time. 
    //If we use append it will fall then become invisble (not at the same time)
}
