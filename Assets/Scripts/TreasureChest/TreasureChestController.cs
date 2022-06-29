using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script to control interaction between player and treasure chest
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
        TreasureChestManager.i.OpenMenu();
        Destroy(gameObject);
        yield return null;
    }

}


