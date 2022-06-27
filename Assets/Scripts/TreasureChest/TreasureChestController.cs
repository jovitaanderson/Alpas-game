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
        treasureChestSpawn.chestDestoryed();
        spriteRenderer.sprite = newSprite;
        //spriteRenderer.sprite = newSprite;
        TreasureChestManager.i.OpenMenu();
        //if answer correctly give coins
        //if answer wrongly no reward
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        yield return TreasureChestManager.i.TreasureChest();

        Destroy(gameObject);
    }

}

public enum ChestState { Idle, Question }


