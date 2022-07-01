using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public enum AudioTypes { soundEffect, music }
    public AudioTypes audioType;
    public string clipName;
    public AudioClip AudioClip;

    [Range(0, 1)]
    public float volume = 0.5f;
}
