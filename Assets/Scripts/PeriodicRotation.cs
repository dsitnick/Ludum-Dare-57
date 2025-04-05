using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicRotation : MonoBehaviour
{

    public Vector3 axis;

    public float frequency, amplitude;

    void Update()
    {
        transform.localEulerAngles = axis * Mathf.Sin(Time.time * 2 * Mathf.PI * frequency) * amplitude;
    }

}
