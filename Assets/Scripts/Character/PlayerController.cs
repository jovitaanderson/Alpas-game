using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public event Action OnEncountered;

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
                StartCoroutine(character.Move(input, CheckForEncounters));
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

    //Probability of encountering an animal in a bush
    private void CheckForEncounters()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.GrassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                //animator.SetBool("isMoving", false);
                character.Animator.IsMoving = false;
                OnEncountered();
            }
        }
    }
}
