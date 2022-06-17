using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItems : MonoBehaviour
{
    public Transform[] SpawnPoints;
    public float spawnTime = 5f;

    public GameObject Coins;

    private void Start()
    {
        InvokeRepeating("SpawnCoins", spawnTime, spawnTime);
    }

    void SpawnCoins()
    {
        //set the index number of the array randomly
        int spawnIndex = Random.Range(0, SpawnPoints.Length);
        Instantiate(Coins, SpawnPoints[spawnIndex].position, SpawnPoints[spawnIndex].rotation);
    }

}
