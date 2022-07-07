using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjects : MonoBehaviour
{
    public static EssentialObjects i { get; private set; }

    private void Awake() 
    {
        i = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Destory()
    {
        Destroy(gameObject);
    } 
}
