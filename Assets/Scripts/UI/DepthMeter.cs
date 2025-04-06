using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class DepthMeter : MonoBehaviour
{
    
    public TMP_Text text;
    public Transform player, sun;
    
    public float currentDepth => sun.transform.position.y - player.transform.position.y;
    
    void Update()
    {
        float d = Mathf.Round(currentDepth * 10) / 10;
        
        text.text = "Depth: " + d;
    }
    
    public void SetDepth(string depthString){
        if (float.TryParse(depthString, out float depth)){
            SetDepth(depth);
        }
    }
    
    public void SetDepth(float depth){
        Vector3 pos = player.transform.position;
        pos.y = sun.transform.position.y - depth;
        player.transform.position = pos;
    }
    
}
