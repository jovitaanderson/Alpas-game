using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] ChoiceBox choiceBox;

    public event Action OnShowDialog;
    public event Action OnDialogFinished;



    //Since dialogmanager will be used for NPC/object(signboards) we use the singleton partten to get the instance
    //and use it in other classes
    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public bool IsShowing { get; private set; }
    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
    }

    public IEnumerator ShowDialog(Dialog dialog, List<string> choices = null, Action<int> onChoiceSelected = null)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);

        foreach (var line in dialog.Lines)
        {
            //play music for each line
            AudioManager.i.PlaySfx(AudioId.UISelect);
            yield return TypeDialog(line);
            yield return new WaitUntil(() => Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM")) || Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM1")));
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        dialogBox.SetActive(false);
        IsShowing = false;
        OnDialogFinished?.Invoke();
    }

    public IEnumerator ShowDialogText(string text, bool waitForInput = true, bool autoClose = true,
        List<string> choices = null, Action<int> onChoiceSelected = null)
    {
        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);

        //play sfx music
        AudioManager.i.PlaySfx(AudioId.UISelect);

        yield return TypeDialog(text);
        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM")) || Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM1")));
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        if (autoClose)
        {
            CloseDialog();
        }
        OnDialogFinished?.Invoke();
    }

    public void HandleUpdate()
    {
    }

    public IEnumerator TypeDialog(string line)
    {
        dialogText.text = "";
        char[] lineCharArray = line.ToCharArray();
       /* foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }*/

        for (int i = 0; i < line.Length; i++)
        {
            dialogText.text += lineCharArray[i];
            yield return new WaitForSeconds(1f / lettersPerSecond);
            //Debug.Log("print");

            if ((Input.GetKey(SettingsManager.i.getKey("CONFIRM")) || Input.GetKey(SettingsManager.i.getKey("CONFIRM1"))) && i >=6)
            {
                //Debug.Log("Return pressed");
                string remainingString = line.Substring(i+1);
                dialogText.text += remainingString;
                i = line.Length - 1;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}