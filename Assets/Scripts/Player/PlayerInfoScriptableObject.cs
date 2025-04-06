using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerInfo", menuName = "Player Info")]
public class PlayerInfoScriptableObject : ScriptableObject
{
    
    public Vector3 position;
    public bool isSubmarine;
    
    public bool isObjectiveComplete;
    
}
