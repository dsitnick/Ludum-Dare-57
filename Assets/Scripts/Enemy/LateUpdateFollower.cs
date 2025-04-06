using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateUpdateFollower : MonoBehaviour
{
    
    public Transform[] followers;
    public Transform[] targets;
    
    private Vector3[] positions;
    private Quaternion[] rotations;
    
    public float posLerp, rotLerp;
    
    void Start(){
        positions = new Vector3[followers.Length];
        rotations = new Quaternion[followers.Length];
        
        for (int i = 0; i < followers.Length; i++){
            positions[i] = targets[i].position;
            rotations[i] = targets[i].rotation;
        }
    }
    
    void LateUpdate(){
        for (int i = 0; i < followers.Length; i++){
            positions[i] = Vector3.Lerp(positions[i], targets[i].position, posLerp * Time.deltaTime);
            rotations[i] = Quaternion.Slerp(rotations[i], targets[i].rotation, rotLerp * Time.deltaTime);
            
            followers[i].position = positions[i];
            followers[i].rotation = rotations[i];
        }
        
    }
    
}
