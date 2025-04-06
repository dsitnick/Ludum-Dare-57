using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class DepthMeter : MonoBehaviour
{
    
    public TMP_Text text;
    public Transform sun;
    
    public PlayerInfoScriptableObject playerInfo;
    
    public float offset;
    
    public float currentDepth => sun.transform.position.y - playerInfo.position.y + offset;
    
    void Update()
    {
        float d = Mathf.Round(currentDepth / 10) * 10;
        
        text.text = "Depth: " + d;
    }
    
}
