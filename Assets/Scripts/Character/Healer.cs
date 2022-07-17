using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform player, Dialog dialog)
    {
        int selectedChoice = 0;

        yield return DialogManager.Instance.ShowDialogText("You look tired! Would you like to rest here?",
            choices: new List<string>() { "Yes", "No" }, 
            onChoiceSelected: (choiceIndex) => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            //choose yes
            yield return Fader.i.FadeIn(0.5f);

            var playerParty = player.GetComponent<AnimalParty>();
            playerParty.Animals.ForEach(p => p.Heal());
            foreach (var animals in playerParty.Animals)
            {
                foreach (var moves in animals.Moves)
                {
                    Debug.Log($"Orginial : {moves.PP} base : { moves.Base.PP}");
                    moves.PP = moves.Base.PP;
                }
            }

            playerParty.PartyUpdated();

            yield return Fader.i.FadeOut(0.5f);
            yield return DialogManager.Instance.ShowDialogText($"Your animals are fully healed!");
        }
        else if (selectedChoice == 1)
        {
            //choose no
            yield return DialogManager.Instance.ShowDialogText($"Okay! Come back if you change your mind!");
        }

        
    }
}
