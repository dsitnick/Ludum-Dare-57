using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicRotation : MonoBehaviour
{

    public Vector3 axis;

    public float frequency, amplitude;
    
    float t = 0;

    void Update()
    {
        t += Time.deltaTime * frequency;
        transform.localEulerAngles = axis * Mathf.Sin(t * 2 * Mathf.PI) * amplitude;
    }

}
