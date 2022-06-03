using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISavable
{
    [SerializeField] new string name;
    [SerializeField] Sprite sprite;

    private Vector2 input;

    private Character character;

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

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Return)){
            Interact();
        }
    }

    void Interact()
    {
        //var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;
        // Debug.DrawLine(transform.position,interactPos,Color.green,0.5f);
        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer); 
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    public string Name{
        get => name;
    }
    public Sprite Sprite{
        get => sprite;
    }

    private void OnMoveOver() 
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayers);

        foreach(var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null) 
            {
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }

    //Used to save data
    public object CaptureState()
    {
        float[] position = new float[] { transform.position.x, transform.position.y };
        return position;
    }

    //Used to restore data when game loaded
    public void RestoreState(object state)
    {
        var position = (float[])state;
        transform.position = new Vector3(position[0], position[1]);
    }

    public Character Character => character;
    
}
