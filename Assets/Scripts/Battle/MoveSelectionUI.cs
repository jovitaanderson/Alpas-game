using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MoveSelectionUI : MonoBehaviour
{
   [SerializeField] List<Text> moveTexts;
   [SerializeField] Color highlightedColor;

   int currentSelection = 0;

   public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove) 
   {
       for (int i = 0; i < currentMoves.Count; ++i)
       {
           moveTexts[i].text = currentMoves[i].Name;
       }

       moveTexts[currentMoves.Count].text = newMove.Name;
   }

   public void HandleMoveSelection(Action<int> onSelected) 
   {
        if (Input.GetKeyDown(SettingsManager.i.getKey("DOWN")))
            ++currentSelection;
        else if (Input.GetKeyDown(SettingsManager.i.getKey("UP")))   
            --currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, AnimalBase.MaxNumOfMoves);

        UpdateMoveSelection(currentSelection);

        if (Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM")))
        {
            onSelected?.Invoke(currentSelection);
        }
   }

   public void UpdateMoveSelection(int selection) 
   {
       for (int i = 0; i < AnimalBase.MaxNumOfMoves + 1; i++)
       {
           if (i == selection)
           {
               moveTexts[i].color = highlightedColor;
           }
           else 
           {
               moveTexts[i].color = Color.black;
           }
       }
   }
}
