using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

/** Saving system with functions tosave/load data to a file
 */

public class SavingSystem : MonoBehaviour
{
    [SerializeField] GameObject saveIcon;
    public static SavingSystem i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    Dictionary<string, object> gameState = new Dictionary<string, object>();

    public void CaptureEntityStates(List<SavableEntity> savableEntities)
    {
        foreach (SavableEntity savable in savableEntities)
        {
            gameState[savable.UniqueId] = savable.CaptureState();
        }
    }

    public void RestoreEntityStates(List<SavableEntity> savableEntities)
    {
        foreach (SavableEntity savable in savableEntities)
        {
            string id = savable.UniqueId;
            if (gameState.ContainsKey(id))
                savable.RestoreState(gameState[id]);
        }
    }

    public void Save(string saveFile)
    {
        CaptureState(gameState);
        SaveFile(saveFile, gameState);
    }

    public void Load(string saveFile)
    {
        gameState = LoadFile(saveFile);
        RestoreState(gameState);
    }

    public void Delete(string saveFile)
    {
        File.Delete(GetPath(saveFile));
    }

    // Used to capture states of all savable objects in the game
    private void CaptureState(Dictionary<string, object> state)
    {
        foreach (SavableEntity savable in FindObjectsOfType<SavableEntity>())
        {
            state[savable.UniqueId] = savable.CaptureState();
        }
    }

    // Used to restore states of all savable objects in the game
    private void RestoreState(Dictionary<string, object> state)
    {
        foreach (SavableEntity savable in FindObjectsOfType<SavableEntity>())
        {
            string id = savable.UniqueId;
            if (state.ContainsKey(id))
                savable.RestoreState(state[id]);
        }
    }

    public void RestoreEntity(SavableEntity entity)
    {
        if (gameState.ContainsKey(entity.UniqueId))
            entity.RestoreState(gameState[entity.UniqueId]);
    }

    void SaveFile(string saveFile, Dictionary<string, object> state)
    {
        StartCoroutine(ShowSavingIcon());
        //todo: for app
        /*string path = GetPath(saveFile);
        print($"saving to {path}");

        using (FileStream fs = File.Open(path, FileMode.Create))
        {
            // Serialize our object
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fs, state);
        }*/

        //todo: for webGL
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        var memoryStream = new MemoryStream();
        using (memoryStream)
        {
            binaryFormatter.Serialize(memoryStream, state);
        }
        string content = Convert.ToBase64String(memoryStream.ToArray());
        PlayerPrefs.SetString("forceSave", content);
        PlayerPrefs.Save();
    }

    Dictionary<string, object> LoadFile(string saveFile)
    {
        //todo: for app
        /*string path = GetPath(saveFile);
        if (!File.Exists(path))
        {
            print($"path doesnt exists");
            return new Dictionary<string, object>();
        }

         using (FileStream fs = File.Open(path, FileMode.Open))
         {
             // Deserialize our object
             BinaryFormatter binaryFormatter = new BinaryFormatter();
             return (Dictionary<string, object>)binaryFormatter.Deserialize(fs);
         }*/

         //todo: for webGL
        
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string temp = PlayerPrefs.GetString("forceSave");
        if (temp == string.Empty)
        {
            return null;
        }
        MemoryStream memoryStream = new MemoryStream(System.Convert.FromBase64String(temp));
        return (Dictionary<string, object>)binaryFormatter.Deserialize(memoryStream);
        
    }

    private string GetPath(string saveFile)
    {
        return Path.Combine(Application.persistentDataPath, saveFile);
    }

    IEnumerator ShowSavingIcon()
    {
        saveIcon.SetActive(true);
        yield return new WaitForSeconds(1f);
        saveIcon.SetActive(false);
    }
}
