using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightSource : MonoBehaviour
{

    public Light[] lights;
    public Transform[] lightColliders;

    public Vector3 lastVisiblePosition;

    public bool isOn;

    [SerializeField] public UnityEvent<bool> onSetActive;


    void Start()
    {
        SetLightActive(isOn);
    }

    public void TurnLightOn() => SetLightActive(true);
    public void TurnLightOff() => SetLightActive(false);

    public void SetLightActive(bool isOn)
    {
        //Debug.Log("Set lights " + name + "  " + isOn);
        this.isOn = isOn;
        foreach (Light l in lights)
        {
            l.enabled = isOn;
        }
        foreach (var c in lightColliders)
        {
            c.localScale = isOn ? Vector3.one : Vector3.zero;
        }
        
        onSetActive.Invoke(isOn);
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger");
        FishAggro target = other.GetComponentInParent<FishAggro>();
        target?.AddLight(this);
    }

    void OnTriggerExit(Collider other)
    {
        FishAggro target = other.GetComponentInParent<FishAggro>();
        target?.RemoveLight(this);
    }

    void FixedUpdate()
    {
        if (isOn)
        {
            lastVisiblePosition = transform.position;
        }
    }

}
