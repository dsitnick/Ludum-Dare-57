using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmSwitcher : MonoBehaviour
{
    
    public AudioSource audioSource;
    
    public AudioClip[] clips;
    
    public void SetAlarmLevel(int level){
        if (level < 0){
            audioSource.Stop();
            return;
        }
        audioSource.clip = clips[level];
        audioSource.Play();
    }
    
}
