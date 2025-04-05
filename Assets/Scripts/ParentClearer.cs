using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentClearer : MonoBehaviour
{
    
    public Transform[] targets;
    
    private Transform root;
    
    void Awake()
    {
        root = new GameObject(name + " Targets").transform;
        foreach (Transform target in targets){
            target.parent = root;
        }
    }
    
    void OnDestroy()
    {
        Destroy(root.gameObject);
    }
}
