using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChestSpawn : MonoBehaviour
{
    public GameObject treasureChest;
    public int maxTreasureChestInMap;
    public Vector2 minPosition; //min x = -59.5, min y = -29.5
    public Vector2 maxPosition; //max x = 79.5 , max y = 29.5

    public float Timer = 0;
    [SerializeField] public float TimeForNextChestSpawn;

    static int currCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currCount < maxTreasureChestInMap)
        {
            Timer += Time.deltaTime;
            if (Timer >= TimeForNextChestSpawn)
            {
                SpawnTreasureChestRandom();
                Timer = 0;
            }
        }
    }

    void SpawnTreasureChestRandom()
    {
        Vector3 randomPoint = GetValidSpawnPoint();
        Instantiate(treasureChest, randomPoint, Quaternion.identity);
        currCount++;
        Debug.Log(currCount);
    }

    Vector3 GetValidSpawnPoint()
    {
        //Vector3 randomPoint = new Vector3(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y), 0);
        int x = Random.Range(Mathf.CeilToInt(minPosition.x), Mathf.FloorToInt(maxPosition.x));
        int y = Random.Range(Mathf.CeilToInt(minPosition.y), Mathf.FloorToInt(maxPosition.y));
        Vector3 randomPoint = new Vector3((float)(x + 0.5), (float)(y + 0.5), 0);


        while (!IsPositionClear(randomPoint))
        {
            x = Random.Range(Mathf.CeilToInt(minPosition.x), Mathf.FloorToInt(maxPosition.x));
            y = Random.Range(Mathf.CeilToInt(minPosition.y), Mathf.FloorToInt(maxPosition.y));
            randomPoint = new Vector3((float)(x + 0.5), (float)(y + 0.5), 0);

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
