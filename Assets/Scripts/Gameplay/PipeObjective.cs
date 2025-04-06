using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeObjective : MonoBehaviour
{
    
    public static HashSet<PipeObjective> objectives = new HashSet<PipeObjective>();
    
    public Transform root, slideRoot, location;
    public Vector3 position => location.position;
    
    void OnEnable(){
        objectives.Add(this);
    }
    void OnDisable(){
        objectives.Remove(this);
    }
    
    public void Setup(Vector3 position, float scale){
        root.position = position;
        switch (Random.Range(0, 3)){
            case 0: root.eulerAngles = Vector3.zero; break;
            case 1: root.eulerAngles = Vector3.up * 90; break;
            case 2: root.eulerAngles = Vector3.right * 90; break;
        }
        slideRoot.localPosition = Vector3.forward * Random.Range(0.25f, 0.5f) * scale * Mathf.Sign(Random.value - 0.5f);
        slideRoot.localEulerAngles = Vector3.forward * Random.Range(-180f, 180f);
    }
    
    public static PipeObjective GetClosestObjective(Vector3 position){
        PipeObjective result = null;
        float distance = float.MaxValue;
        foreach (PipeObjective objective in objectives){
            float d = Vector3.Distance(objective.position, position);
            if (d < distance){
                distance = d;
                result = objective;
            }
        }
        return result;
    }
    
}
