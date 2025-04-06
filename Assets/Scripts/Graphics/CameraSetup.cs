using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Rendering.Experimental.LookDev;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    

    void OnEnable()
    {
        Camera cam = GetComponent<Camera>();
        if (cam != null) cam.depthTextureMode |= DepthTextureMode.Depth;
    }

}