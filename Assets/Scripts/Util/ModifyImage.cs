using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModifyImage : MonoBehaviour
{
    public Image image;
    public float alphaValue;
    public Color color;

    void Start()
    {
        image = GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = alphaValue;
        image.color = tempColor;
        if(alphaValue ==1)
            image.color = color;
    }
}
