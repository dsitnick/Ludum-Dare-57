using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentClearer : MonoBehaviour
{
    
    public Transform[] targets;
    
    private GameObject root;
    
    void Awake()
    {
        root = new GameObject(name + " Targets");
        foreach (Transform target in targets){
            target.parent = root.transform;
        }
    }
    
    void OnDestroy()
    {
        if (root != null) Destroy(root);
    }
}
