using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmSwitcher : MonoBehaviour
{
    public Animator animator;
    
    public void SetAlarmLevel(int level){
        animator.SetFloat("Alarm Level", level + 1);
    }
    
}
