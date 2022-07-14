using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float moveSpeed; // variable set a public so we can edit it on unity editor


    public bool IsMoving { get; private set; }

    public float OffsetY { get; private set; } = 0.3f;

    CharacterAnimator animator;
    private void Awake()
    {
        animator = GetComponent <CharacterAnimator>();
        if(GetComponent<Merchant>() == null)
        {
            SetPositionAndSnapToTile(transform.position);
        }
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        //2.3 -> floor -> 2 -> 2.5
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;

        transform.position = pos;

    }
    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver=null)
    {
        animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        var ledge = CheckForLedge(targetPos);
        if(ledge != null)
        {
            if(ledge.TryToJump(this,moveVec))
                yield break;
        }

        if (!IsPathClear(targetPos))
            yield break;

        //surfing
        if (animator.IsSurfing && Physics2D.OverlapCircle(targetPos, 0.3f, GameLayers.i.WaterLayer) == null)
            animator.IsSurfing = false;

        IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                //this.Animator.IsRunning = true;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, (moveSpeed + 2) * Time.deltaTime);
            }
            else
            {
                //this.Animator.IsRunning = false;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            }
            yield return null;
        }
        transform.position = targetPos;

        IsMoving = false;
        //this.Animator.IsRunning = false;

        //null conidition operator, if OnMoveOver is null, we wont call(invoke) it
        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;

        var collisionLayer = GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer;
        if (!animator.IsSurfing)
            collisionLayer = collisionLayer | GameLayers.i.WaterLayer;

        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, collisionLayer) == true)
            return false; 
        return true;

    }

    //If object is solidObjectLayer, player cannot walk over it
    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer) != null)
        {
            return false;
        }
        return true;
    }

    Ledge CheckForLedge(Vector3 targetPos)
    {
        var collider = Physics2D.OverlapCircle(targetPos, 0.15f, GameLayers.i.LedgeLayer);
        return collider?.GetComponent<Ledge>();
    }

    public void LookTowards(Vector3 targetPos)
    {
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        //remove if we want diagonally (prob dont need)
       // if (xdiff == 0 || ydiff == 0)
      //  {
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
       // }
       // else
        //    Debug.LogError("Error in Look Towards:You can't ask the character to look diagonally");
    }

    public CharacterAnimator Animator {
        get => animator;
    }
}
