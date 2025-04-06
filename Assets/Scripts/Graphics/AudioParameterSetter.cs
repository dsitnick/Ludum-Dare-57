using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioParameterSetter : MonoBehaviour
{
    
    public AudioMixer audioMixer;
    
    [Range(0, 1)]
    public float filterCutoff;
    public float minFilterFreq = 20, maxFilterFreq = 22000;
    
    void Start(){
        SetFilterCutoff();
    }
    
    float lastFilterCutoff;
    void Update(){
        if (lastFilterCutoff != filterCutoff){
            lastFilterCutoff = filterCutoff;
            SetFilterCutoff();
        }
    }
    
    public void SetFilterCutoff(){
        audioMixer.SetFloat("Master LP Cutoff", Mathf.Lerp(minFilterFreq, maxFilterFreq, filterCutoff));
    }
    
}
