using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum ControlState { Unselected, Selected}

public class KeybindManager : MonoBehaviour
{
    [SerializeField] Text messageText;
    public GameObject[] keybindButtons;
    [SerializeField] GameObject keybindUI;
    public event Action onBack;

    public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    private GameObject currentKey;
    ControlState state;

    private Color32 normal = new Color32(255, 255, 255, 255);
    //private Color32 selected = new Color32(239, 116, 36, 255);

    public static KeybindManager i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("SavedKeybinds1"))
        {
            setDefaultKeybinds();
        }

        keys.Add("UP", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("UP", "UpArrow")));
        keys.Add("LEFT", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LEFT", "LeftArrow")));
        keys.Add("DOWN", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("DOWN", "DownArrow")));
        keys.Add("RIGHT", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RIGHT", "RightArrow")));
        keys.Add("SPRINT", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("SPRINT", "LeftShift")));

        keys.Add("MENU", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MENU", "M")));
        keys.Add("CONFIRM", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("CONFIRM", "Return")));
        keys.Add("BACK", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("BACK", "Escape")));
        messageText.text = "";
        updateAllKeyText();
    }

    public void setDefaultKeybinds()
    {
        PlayerPrefs.SetString("SavedKeybinds1", "true");
        PlayerPrefs.SetString("UP", "UpArrow");
        PlayerPrefs.SetString("LEFT", "LeftArrow");
        PlayerPrefs.SetString("DOWN", "DownArrow");
        PlayerPrefs.SetString("RIGHT", "RightArrow");
        PlayerPrefs.SetString("SPRINT", "LeftShift");
        PlayerPrefs.SetString("MENU", "M");
        PlayerPrefs.SetString("CONFIRM", "Return");
        PlayerPrefs.SetString("BACK", "Escape");
        updateAllKeyText();
        messageText.text = "Default keybinds set";
        messageText.color = new Color32(0, 0, 0, 255);
    }

    private void OnGUI()
    {
        if (GetComponent<GameController>().State == GameState.Controls)
        {
            if (Input.GetKeyDown(KeybindManager.i.keys["BACK"]) && currentKey == null)
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
            keyButton.GetComponentInChildren<Text>().text = PlayerPrefs.GetString(keyButton.name);
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

    public void SaveKeys()
    {
        if (keys.ContainsValue(KeyCode.None))
        {
            messageText.text = "Cannot save. All keys must be binded";
            messageText.color = new Color32(255, 25, 25, 255);
        }
        else
        {
            messageText.text = "";
            foreach (var key in keys)
            {
                PlayerPrefs.SetString(key.Key, key.Value.ToString());
            }
            PlayerPrefs.Save();
            messageText.text = "Saved";
            messageText.color = new Color32(0, 0, 0, 255);
        }
    }

    public void openKeybindUI()
    {
        updateAllKeyText();
        keybindUI.SetActive(true);

    }

    public void closeKeybindUI()
    {
        messageText.text = "";
        keybindUI.SetActive(false);
    }
}
