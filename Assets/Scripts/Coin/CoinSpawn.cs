using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawn : MonoBehaviour
{
    public GameObject coin;
    public int maxCoinInMap;
    public Vector2 minPosition; //min x = -59.5, min y = -29.5
    public Vector2 maxPosition; //max x = 79.5 , max y = 29.5
    public Collider2D[] colliders;
    public float radius;

    public float Timer = 0;
    [SerializeField] public float TimeForNextCoinSpawn;

    static int currCount = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currCount < maxCoinInMap)
        {
            Timer += Time.deltaTime;
            if (Timer >= TimeForNextCoinSpawn)
            {
                SpawnTreasureChestRandom();
                Timer = 0;
            }
        }

        


    }

    void SpawnTreasureChestRandom()
    {
        Vector3 randomPoint = GetValidSpawnPoint();
        Instantiate(coin, randomPoint, Quaternion.identity);
        currCount++;
        Debug.Log(currCount);
    }

    Vector3 GetValidSpawnPoint()
    {
        Vector3 randomPoint = new Vector3(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y), 0);
        while (!IsPositionClear(randomPoint))
        {
            randomPoint = new Vector3(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y), 0);
        }
        return randomPoint;
    }

    private bool IsPositionClear(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer | GameLayers.i.GrassLayer | GameLayers.i.NoSpawnLayer) != null)
        {
            return false;
        }
        return true;

    }

    public void chestDestoryed()
    {
        currCount--;
    }
}