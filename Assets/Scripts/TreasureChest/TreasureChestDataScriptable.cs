using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TreasureChestData", menuName = "TreasureChestData")]
public class TreasureChestDataScriptable : ScriptableObject
{
    public List<TreasureChestQuestion> questions;
}
