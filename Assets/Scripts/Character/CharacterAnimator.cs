using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;

    [SerializeField] List<Sprite> walkDownLeftSprites;
    [SerializeField] List<Sprite> walkDownRightSprites;
    [SerializeField] List<Sprite> walkUpLeftSprites;
    [SerializeField] List<Sprite> walkUpRightSprites;

    // Parameters
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }

    // States
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    //Diagonal states
    SpriteAnimator walkDownLeftAnim;
    SpriteAnimator walkDownRightAnim;
    SpriteAnimator walkUpLeftAnim;
    SpriteAnimator walkUpRightAnim;

    SpriteAnimator currentAnim;
    bool wasPreviouslyMoving;

    // Refrences
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);

        walkDownLeftAnim = new SpriteAnimator(walkDownLeftSprites, spriteRenderer);
        walkDownRightAnim = new SpriteAnimator(walkDownRightSprites, spriteRenderer);
        walkUpLeftAnim = new SpriteAnimator(walkUpLeftSprites, spriteRenderer);
        walkUpRightAnim = new SpriteAnimator(walkUpRightSprites, spriteRenderer);

        SetFacingDirection(defaultDirection);

        currentAnim = walkDownAnim;
    }

    private void Update()
    {
        var prevAnim = currentAnim;
        if (MoveX == 1)
            if (MoveY == 1)
                currentAnim = walkUpRightAnim;
            else if (MoveY == -1)
                currentAnim = walkDownRightAnim;
            else currentAnim = walkRightAnim;

        else if (MoveX == -1)
            if (MoveY == 1)
                currentAnim = walkUpLeftAnim;
            else if (MoveY == -1)
                currentAnim = walkDownLeftAnim;
            else currentAnim = walkLeftAnim;

        else if (MoveY == 1)
            if (MoveX == 1)
                currentAnim = walkUpRightAnim;
            else if (MoveX == -1)
                currentAnim = walkUpLeftAnim;
            else currentAnim = walkUpAnim;
        else if (MoveY == -1)
            if (MoveX == 1)
                currentAnim = walkDownRightAnim;
            else if (MoveX == -1)
                currentAnim = walkDownLeftAnim;
            else currentAnim = walkDownAnim;

        if (currentAnim != prevAnim || IsMoving != wasPreviouslyMoving)
            currentAnim.Start();

        if (IsMoving)
            currentAnim.HandleUpdate();
        else
            //TODO: make idle animations
            //if(currentAnim == walkLeftAnim) { walkLeftAnim.Start(); }

            spriteRenderer.sprite = currentAnim.Frames[0];


        wasPreviouslyMoving = IsMoving;
    }

    public void SetFacingDirection(FacingDirection dir)
    {
        if (dir == FacingDirection.Right)
            MoveX = 1;
        else if (dir == FacingDirection.Left)
            MoveX = -1;
        else if (dir == FacingDirection.Down)
            MoveY = -1;
        else if (dir == FacingDirection.Up)
            MoveY = 1;
    }

    public FacingDirection DefaultDirection
    {
        get => defaultDirection;
    }
}

public enum FacingDirection { Up, Down, Left, Right }

