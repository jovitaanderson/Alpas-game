using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class ControlManager : MonoBehaviour
{
    [SerializeField] Text messageText;
    public GameObject[] keybindButtons;
    [SerializeField] GameObject keybindUI;
    public event Action onBack;

    public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
    public Dictionary<string, KeyCode> keys1 = new Dictionary<string, KeyCode>();

    private GameObject currentKey;

    private Color32 normal = new Color32(255, 255, 255, 255);

    public static ControlManager i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("SavedKeybinds0"))
        {
            setDefaultKeybinds();
        }
        //Actions
        keys.Add("MENU", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MENU", "M")));
        keys1.Add("MENU1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MENU1", "None")));
        keys.Add("CONFIRM", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("CONFIRM", "Return")));
        keys1.Add("CONFIRM1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("CONFIRM1", "Space")));
        keys.Add("BACK", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("BACK", "Escape")));
        keys1.Add("BACK1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("BACK1", "Backspace")));

        //Selector controls
        keys.Add("UP", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("UP", "UpArrow")));
        keys1.Add("UP1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("UP1", "W")));
        keys.Add("LEFT", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LEFT", "LeftArrow")));
        keys1.Add("LEFT1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LEFT1", "A")));
        keys.Add("DOWN", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("DOWN", "DownArrow")));
        keys1.Add("DOWN1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("DOWN1", "S")));
        keys.Add("RIGHT", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RIGHT", "RightArrow")));
        keys1.Add("RIGHT1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RIGHT1", "D")));
        
        //movement controls cannot be changed

        messageText.text = "test";
        updateAllKeyText();
    }

    public KeyCode getKey(string key)
    {
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(key));
    }

    public void setDefaultKeybinds()
    {
        PlayerPrefs.SetString("SavedKeybinds1", "true");
        PlayerPrefs.SetString("UP", "UpArrow");
        PlayerPrefs.SetString("UP1", "W");
        PlayerPrefs.SetString("LEFT", "LeftArrow");
        PlayerPrefs.SetString("LEFT1", "A");
        PlayerPrefs.SetString("DOWN", "DownArrow");
        PlayerPrefs.SetString("DOWN1", "S");
        PlayerPrefs.SetString("RIGHT", "RightArrow");
        PlayerPrefs.SetString("RIGHT1", "D");
        PlayerPrefs.SetString("SPRINT", "LeftShift");
        PlayerPrefs.SetString("SPRINT1", "RightShift");
        PlayerPrefs.SetString("MENU", "M");
        PlayerPrefs.SetString("MENU1", "None");
        PlayerPrefs.SetString("CONFIRM", "Return");
        PlayerPrefs.SetString("CONFIRM1", "Space");
        PlayerPrefs.SetString("BACK", "Escape");
        PlayerPrefs.SetString("BACK1", "Backspace");
        updateAllKeyText();
        messageText.text = "Default keybinds set";
        messageText.color = new Color32(0, 0, 0, 255);
    }

    private void OnGUI()
    {
        if (GetComponent<GameController>().State == GameState.Controls)
        {
            if (Input.GetKeyDown(ControlManager.i.getKey("BACK")) && currentKey == null)
                onBack?.Invoke();

            if (currentKey != null)
            {
                Event e = Event.current;
                if (e.isKey)
                {
                    if (keys.ContainsValue(e.keyCode) || keys1.ContainsValue(e.keyCode))
                    {
                        if (keys.ContainsValue(e.keyCode))
                        {
                            string myKey = keys.FirstOrDefault(x => x.Value == e.keyCode).Key;
                            keys[myKey] = KeyCode.None;
                            updateKeyText(myKey, keys[myKey]);
                        }
                        else if (keys1.ContainsValue(e.keyCode))
                        {
                            string myKey = keys1.FirstOrDefault(x => x.Value == e.keyCode).Key;
                            keys1[myKey] = KeyCode.None;
                            updateKeyText(myKey, keys1[myKey]);
                        }

                    }

                    if (currentKey.name.Contains("1"))
                    {
                        keys1[currentKey.name] = e.keyCode;
                        updateKeyText(currentKey.name, keys1[currentKey.name]);
                    }
                    else
                    {
                        keys[currentKey.name] = e.keyCode;
                        updateKeyText(currentKey.name, keys[currentKey.name]);
                    }

                    currentKey.GetComponent<Image>().color = normal;
                    currentKey = null;
                }
            }
        }
    }

    public void updateKeyText(string key, KeyCode code)
    {
        Text temp = Array.Find(keybindButtons, x => x.name == key).GetComponentInChildren<Text>();
        temp.text = code.ToString();
    }

    public void updateAllKeyText()
    {
        foreach (var keyButton in keybindButtons)
        {
            keyButton.GetComponentInChildren<Text>().text = PlayerPrefs.GetString(keyButton.name);
        }
    }

    public void CannotChangeMessageText()
    {
        messageText.text = "Movement keys cannot be changed";
        messageText.color = new Color32(255, 25, 25, 255);
    }


    public void ChangeKey(GameObject clicked)
    {
        if (currentKey != null)
        {
            currentKey.GetComponent<Image>().color = normal;
        }
        currentKey = clicked;
        currentKey.GetComponent<Image>().color = GlobalSettings.i.HighlightedColor;
    }

    public void SaveKeys()
    {
        if (keys.ContainsValue(KeyCode.None))
        {
            messageText.text = "Cannot save. All main keys must be binded";
            messageText.color = new Color32(255, 25, 25, 255);
        }
        else
        {
            foreach (var key in keys)
            {
                PlayerPrefs.SetString(key.Key, key.Value.ToString());
            }
            foreach (var key1 in keys1)
            {
                PlayerPrefs.SetString(key1.Key, key1.Value.ToString());
            }
            PlayerPrefs.Save();
            messageText.text = "Saved";
            messageText.color = new Color32(0, 0, 0, 255);
        }
    }

    public void openControlsUI()
    {
        updateAllKeyText();
        keybindUI.SetActive(true);
    }

    public void closeControlsUI()
    {
        messageText.text = "";
        keybindUI.SetActive(false);
    }
}
