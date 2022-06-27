using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestUI : MonoBehaviour
{
    [SerializeField] Image chestImage;
    [SerializeField] Image backgroundImage;
    [SerializeField] Text chestText;

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            chestText.color = Color.white;
            backgroundImage.color = GlobalSettings.i.HighlightedColor;
        }

        else
        {
            chestText.color = Color.black;
            backgroundImage.color = Color.white;
        }
    }
}
