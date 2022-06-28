using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageMemberUI : MonoBehaviour
{

    [SerializeField] Image image;
    [SerializeField] Image imageBorder;
    [SerializeField] GameObject lvlUpObj;

    Animal _animal;
    public void Init(Animal animal)
    {
        _animal = animal;
        UpdateData();

        _animal.OnHPChanged += UpdateData;

    }

    void UpdateData()
    {
        image.sprite = _animal.Base.FrontSprite;
        checkLvlUpImage();
    }

    public void checkLvlUpImage()
    {
        var evolution = _animal.CheckForEvolution();
        if (evolution != null)
            lvlUpObj.SetActive(true);
        else
            lvlUpObj.SetActive(false);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            var borderColor = imageBorder.color;
            borderColor.a = 1f;
            imageBorder.color = borderColor;
        }

        else
        {
            var borderColor = imageBorder.color;
            borderColor.a = 0f;
            imageBorder.color = borderColor;
        }
    }
}
