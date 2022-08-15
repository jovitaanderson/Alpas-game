using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] new string name;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    [SerializeField] string trainerAppearsClip; //a trainer appears(bad guy version)

    //State
    bool battleLost = false;

    public bool BattleLostState => battleLost;

    Character character;
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        if (fov != null)
        {
            SetFovRotation(character.Animator.DefaultDirection);
        }
    }

    private void Update()
    {
        character.HandleUpdate();
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (!battleLost)
        {
            if (initiator.GetComponent<AnimalParty>().GetHealthyPPAnimal() != null)
            {
                character.LookTowards(initiator.position);
                AudioManager.i.Play(trainerAppearsClip);
                yield return DialogManager.Instance.ShowDialog(dialog);
                GameController.Instance.StartTrainerBattle(this);
            }
            else 
                yield return DialogManager.Instance.ShowDialogText($"All your animals are fainted, cannot fight with {name}.");
           
        }
        else
        {
            character.LookTowards(initiator.position);
            yield return DialogManager.Instance.ShowDialog(dialogAfterBattle);
        }

    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        AudioManager.i.Play(trainerAppearsClip);

        // Show Exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        // Walk towards the player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        // Show dialog
        yield return DialogManager.Instance.ShowDialog(dialog);
        GameController.Instance.StartTrainerBattle(this);
        
    }

    public void BattleLost()
    {
        battleLost = true;
        if (fov != null)
        {
            fov.gameObject.SetActive(false);
        }

        //TODO: if win battle, unlock next trainer
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle =0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270;
        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public object CaptureState()
    {
        return battleLost;
    }

    public void RestoreState(object state)
    {
        battleLost = (bool)state;
        if (battleLost)
            fov.gameObject.SetActive(false);
        
    }

    public string Name {
        get => name;
    }
    public Sprite Sprite {
        get => sprite;
    }
}
