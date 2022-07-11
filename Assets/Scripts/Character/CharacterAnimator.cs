using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;
    [Header("Walk Animation")]
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;

    [SerializeField] List<Sprite> walkDownLeftSprites;
    [SerializeField] List<Sprite> walkDownRightSprites;
    [SerializeField] List<Sprite> walkUpLeftSprites;
    [SerializeField] List<Sprite> walkUpRightSprites;

    [SerializeField] List<Sprite> surfSprites;

    [Header("Run Animation")]
    [SerializeField] List<Sprite> runDownSprites;
    [SerializeField] List<Sprite> runUpSprites;
    [SerializeField] List<Sprite> runRightSprites;
    [SerializeField] List<Sprite> runLeftSprites;

    [SerializeField] List<Sprite> runDownLeftSprites;
    [SerializeField] List<Sprite> runDownRightSprites;
    [SerializeField] List<Sprite> runUpLeftSprites;
    [SerializeField] List<Sprite> runUpRightSprites;

    // Parameters
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }
    public bool IsRunning { get; set; }
    public bool IsJumping { get; set; }
    public bool IsSurfing { get; set; }

    // Walk states
    SpriteAnimator walkDownAnim, walkUpAnim, walkRightAnim, walkLeftAnim;
    //Diagonal
    SpriteAnimator walkDownLeftAnim, walkDownRightAnim, walkUpLeftAnim, walkUpRightAnim;

    //Run states
    SpriteAnimator runDownAnim, runUpAnim, runRightAnim, runLeftAnim;
    //Diagonal
    SpriteAnimator runDownLeftAnim, runDownRightAnim, runUpLeftAnim, runUpRightAnim;

    SpriteAnimator currentAnim;
    bool wasPreviouslyMoving;

    // Refrences
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        //States
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);

        //Diagonal states
        walkDownLeftAnim = new SpriteAnimator(walkDownLeftSprites, spriteRenderer);
        walkDownRightAnim = new SpriteAnimator(walkDownRightSprites, spriteRenderer);
        walkUpLeftAnim = new SpriteAnimator(walkUpLeftSprites, spriteRenderer);
        walkUpRightAnim = new SpriteAnimator(walkUpRightSprites, spriteRenderer);

        //Run states
        runDownAnim = new SpriteAnimator(runDownSprites, spriteRenderer);
        runUpAnim = new SpriteAnimator(runUpSprites, spriteRenderer);
        runRightAnim = new SpriteAnimator(runRightSprites, spriteRenderer);
        runLeftAnim = new SpriteAnimator(runLeftSprites, spriteRenderer);

        //Diagonal states
        runDownLeftAnim = new SpriteAnimator(runDownLeftSprites, spriteRenderer);
        runDownRightAnim = new SpriteAnimator(runDownRightSprites, spriteRenderer);
        runUpLeftAnim = new SpriteAnimator(runUpLeftSprites, spriteRenderer);
        runUpRightAnim = new SpriteAnimator(runUpRightSprites, spriteRenderer);

        SetFacingDirection(defaultDirection);

        currentAnim = walkDownAnim;
    }

    private void Update()
    {
        var prevAnim = currentAnim;

        //surfing
        if (!IsSurfing)
        {
            if (MoveX == 1 && MoveY == 1)
                currentAnim = IsRunning ? runUpRightAnim : walkUpRightAnim;
            else if (MoveX == 1 && MoveY == -1)
                currentAnim = IsRunning ? runDownRightAnim : walkDownRightAnim;
            else if (MoveX == -1 && MoveY == 1)
                currentAnim = IsRunning ? runUpLeftAnim : walkUpLeftAnim;
            else if (MoveX == -1 && MoveY == -1)
                currentAnim = IsRunning ? runDownLeftAnim : walkDownLeftAnim;
            else if (MoveX == 1 && MoveY == 0)
                currentAnim = IsRunning ? runRightAnim : walkRightAnim;
            else if (MoveX == -1 && MoveY == 0)
                currentAnim = IsRunning ? runLeftAnim : walkLeftAnim;
            else if (MoveX == 0 && MoveY == 1)
                currentAnim = IsRunning ? runUpAnim : walkUpAnim;
            else if (MoveX == 0 && MoveY == -1)
                currentAnim = IsRunning ? runDownAnim : walkDownAnim;

            if (currentAnim != prevAnim || IsMoving != wasPreviouslyMoving)
                currentAnim.Start();

            if (IsJumping)
                spriteRenderer.sprite = currentAnim.Frames[currentAnim.Frames.Count - 1]; //sets the last frame of walking animation as a jumping animation
            else if (IsMoving)
                currentAnim.HandleUpdate();
            else
                //TODO: make idle animations
                //if(currentAnim == walkLeftAnim) { walkLeftAnim.Start(); }

                spriteRenderer.sprite = currentAnim.Frames[0];

        } 
        else
        {
            if (MoveX == 1)
                spriteRenderer.sprite = surfSprites[2];
            else if (MoveX == -1)
                spriteRenderer.sprite = surfSprites[3];
            else if (MoveY == 1)
                spriteRenderer.sprite = surfSprites[1];
            else if (MoveY == -1)
                spriteRenderer.sprite = surfSprites[0];
        }

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

