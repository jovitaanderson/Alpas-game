using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AllMusic : ScriptableObject
{
    public Sound[] sounds;

    public int SoundCount
    {
        get
        {
            return sounds.Length;
        }
    }

    public Sound GetSoundIndex(int index)
    {
        return sounds[index];
    }

    public Sound GetSound(string clipname)
    {
        for (int s = 0; s < sounds.Length; s++)
        {
            if (GetSoundIndex(s).clipName.ToString() == clipname)
            {
                return sounds[s];
            }
        }
        return null;
    }
}
