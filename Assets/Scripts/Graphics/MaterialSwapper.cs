using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwapper : MonoBehaviour
{
    
    public Material offMaterial, onMaterial;
    
    public MeshRenderer[] renderers;
    
    public void SetMaterial(bool isOn){
        foreach (MeshRenderer r in renderers) r.material = isOn ? onMaterial : offMaterial;
    }
    
}
