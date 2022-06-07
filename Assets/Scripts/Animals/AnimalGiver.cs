using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalGiver : MonoBehaviour, ISavable
{
    [SerializeField] Animal animalToGive;
    [SerializeField] Dialog dialog;

    bool used = false;

    public IEnumerator GiveAnimal(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);

        animalToGive.Init();
        player.GetComponent<AnimalParty>().AddAnimal(animalToGive);

        used = true;

        string dialogtext = $"{player.Name} received {animalToGive.Base.Name}";
        yield return DialogManager.Instance.ShowDialogText(dialogtext);
    }

    public bool CanBeGiven()
    {
        return animalToGive != null && !used;
    }

    public object CaptureState()
    {
        return used;
    }

    public void RestoreState(object state)
    {
        used = (bool)state;
    }
}
