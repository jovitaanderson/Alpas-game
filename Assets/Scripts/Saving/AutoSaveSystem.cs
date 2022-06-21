using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSaveSystem : MonoBehaviour
{

    public float Timer = 0;
    [SerializeField] public float TimeCheck; 
    
    // Start is called before the first frame update
    void Start()
    {
        //TODO: uncomment this once we done debugging
        //SavingSystem.i.Load("saveSlot1");
    }

    // Update is called once per frame
    void Update()
    {
        Timer = Timer + 1 * Time.deltaTime;
        if(Timer >= TimeCheck)
        {
            if (!PlayerPrefs.HasKey("SavedGame"))
                PlayerPrefs.SetString("SavedGame", "saveSlot1");
            SavingSystem.i.Save("saveSlot1");
            Timer = 0;
        }
        
    }
}
