using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum ControlState { Unselected, Selected}

public class KeybindManager : MonoBehaviour
{
    [SerializeField] Text errorMessage;
    public GameObject[] keybindButtons;
    [SerializeField] GameObject keybindUI;
    public event Action onBack;

    private Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    private GameObject currentKey;
    ControlState state;

    private Color32 normal = new Color32(255, 255, 255, 255);
    //private Color32 selected = new Color32(239, 116, 36, 255);

    // Start is called before the first frame update
    void Start()
    {
        keys.Add("UP", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("UP", "W")));
        keys.Add("LEFT", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LEFT", "A")));
        keys.Add("DOWN", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("DOWN", "S")));
        keys.Add("RIGHT", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RIGHT", "D")));
        keys.Add("SPRINT", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("SPRINT", "RightShift")));

        keys.Add("MENU", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MENU", "M")));
        keys.Add("CONFIRM", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("CONFIRM", "Return")));
        keys.Add("BACK", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("BACK", "Escape")));
        errorMessage.text = "";
        updateAllKeyText();
    }

    private void OnGUI()
    {
        if (GetComponent<GameController>().State == GameState.Controls)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && currentKey == null)
                onBack?.Invoke();

            if (currentKey != null)
            {
                Event e = Event.current;
                if (e.isKey)
                {
                    if (keys.ContainsValue(e.keyCode))
                    {
                        string myKey = keys.FirstOrDefault(x => x.Value == e.keyCode).Key;
                        keys[myKey] = KeyCode.None;
                        updateKeyText(myKey, keys[myKey]);

                    }

                    keys[currentKey.name] = e.keyCode;
                    updateKeyText(currentKey.name, keys[currentKey.name]);
                    currentKey.GetComponent<Image>().color = normal;
                    currentKey = null;
                    state = ControlState.Unselected;
                }
            }
        }
    }

    public void updateAllKeyText()
    {
        foreach (var keyButton in keybindButtons)
        {
            //keyButton.GetComponentInChildren<Text>().text = keys[keyButton.name].ToString();
            keyButton.GetComponentInChildren<Text>().text = PlayerPrefs.GetString(keyButton.name);
            //keyButton.GetComponentInChildren<Text>().text = "wee";
        }
    }

    public void updateKeyText(string key, KeyCode code)
    {
        Text temp = Array.Find(keybindButtons, x => x.name == key).GetComponentInChildren<Text>();
        temp.text = code.ToString();
    }

    public void ChangeKey(GameObject clicked)
    {
        if (currentKey != null)
        {
            currentKey.GetComponent<Image>().color = normal;
        }
        currentKey = clicked;
        currentKey.GetComponent<Image>().color = GlobalSettings.i.HighlightedColor;
        state = ControlState.Selected;
    }

    public void HandleUpdate(Action onBack)
    {
        Debug.Log(currentKey);
        Debug.Log(state);
        if (state == ControlState.Unselected)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                onBack?.Invoke();

        }

    }

    public void SaveKeys()
    {
        if (keys.ContainsValue(KeyCode.None))
        {
            errorMessage.text = "cannot save, all keys must be binded";
            Debug.Log("cannot save, all keys must be binded");
        }
        else
        {
            errorMessage.text = "";
            foreach (var key in keys)
            {
                PlayerPrefs.SetString(key.Key, key.Value.ToString());
            }
            PlayerPrefs.Save();
        }
    }

    public void openKeybindUI()
    {
        updateAllKeyText();
        keybindUI.SetActive(true);

    }

    public void closeKeybindUI()
    {
        keybindUI.SetActive(false);
    }
}
