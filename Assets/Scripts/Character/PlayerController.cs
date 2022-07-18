using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISavable
{
    private String userName;
    [SerializeField] new string name;
    [SerializeField] Sprite sprite;

    private Vector2 input;

    private Character character;

    private void Start()
    {
        if (PlayerPrefs.HasKey("user_name"))
        {
            userName = PlayerPrefs.GetString("user_name");
        }
        else
        {
            userName = name;
        }
    }

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal"); //sets vector x=-1 , when press left or A, sets x=1, when press right, D
            input.y = Input.GetAxisRaw("Vertical"); //sets vector y=-1, when press down or S, sets y =1, when press up or W
            //no pressing of WASD, up,down,left,right, x=0 , y=0

            //prevent diagonal movement
            //if (input.x != 0) { input.y = 0; }

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }

        

        if (Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM")) || Input.GetKeyDown(SettingsManager.i.getKey("CONFIRM1")))
        {
            StartCoroutine(Interact());
        }

        character.HandleUpdate();
    }

    //for collecting coins
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coins"))
        {
            Destroy(collision.gameObject);
        }
    }


    IEnumerator Interact()
    {
        //var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;
        // Debug.DrawLine(transform.position,interactPos,Color.green,0.5f);
        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer | GameLayers.i.WaterLayer);
        if (collider != null)
        {
            character.stopMoving();
            yield return collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    public string UserName{
        get => userName;
    }
    public Sprite Sprite{
        get => sprite;
    }

    IPlayerTriggerable currentlyInTrigger;
    private void OnMoveOver() 
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayers);
        IPlayerTriggerable triggerable = null;

        foreach(var collider in colliders)
        {
            triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null) 
            {
                if (triggerable == currentlyInTrigger && !triggerable.TriggerRepeatedly)
                    break;

                triggerable.OnPlayerTriggered(this);
                currentlyInTrigger = triggerable;
                break;
            }
        }

        if (colliders.Count() == 0 || triggerable != currentlyInTrigger)
            currentlyInTrigger = null;
    }

    //Used to save data
    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            //Save player position
            position = new float[] { transform.position.x, transform.position.y },
            //Save animal party
            animals = GetComponent<AnimalParty>().Animals.Select(p => p.GetSaveData()).ToList(),
            storage = GetComponent<AnimalStorage>().Animals.Select(p => p.GetSaveData()).ToList()
        };
        return saveData ;
    }

    //Used to restore data when game loaded
    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;

        //Restore position
        var pos = saveData.position;
        transform.position = new Vector3(pos[0], pos[1]);

        //Restore Party
        GetComponent<AnimalParty>().Animals = saveData.animals.Select(s => new Animal(s)).ToList();
        GetComponent<AnimalStorage>().Animals = saveData.storage.Select(s => new Animal(s)).ToList();
    }

    public Character Character => character;
}

[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<AnimalSaveData> animals;
    public List<AnimalSaveData> storage;
}
