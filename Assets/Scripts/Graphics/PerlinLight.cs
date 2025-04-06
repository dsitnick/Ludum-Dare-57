using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinLight : MonoBehaviour
{
    
    public float speed, amplitude;
    
    public Light targetLight;
    
    private float intensity;
    
    void Start(){
        intensity = targetLight.intensity;
    }
    
    void Update(){
        float perlin = (Mathf.PerlinNoise(Time.time * speed, 0) - 0.5f) * amplitude;
        
        targetLight.intensity = intensity * (1 + perlin);
    }
}
