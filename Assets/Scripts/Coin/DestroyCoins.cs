using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCoins : MonoBehaviour
{
    //Todo: to change this to a longer time
    public float destroyTime = 10f;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
