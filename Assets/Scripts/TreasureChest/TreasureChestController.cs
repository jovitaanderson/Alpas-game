using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChestController : MonoBehaviour, Interactable
{
    [SerializeField] TreasureChestSpawn treasureChestSpawn;
    public Sprite newSprite;

    private SpriteRenderer spriteRenderer;
    
    public void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        //open chest
        treasureChestSpawn.chestDestoryed();
        spriteRenderer.sprite = newSprite;
        //todo: insert IQ questions
        //yield return DialogManager.Instance.ShowDialogText("insert IQ question");
        
        //if answer correctly give coins
        yield return TreasureChestManager.i.TreasureChest();
        //if answer wrongly no reward

        //destory chest
        Destroy(gameObject);
    }

}

public enum ChestState { Idle, Question }


