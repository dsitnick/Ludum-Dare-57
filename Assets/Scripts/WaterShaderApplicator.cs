using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Rendering.Experimental.LookDev;
using UnityEngine;

[ExecuteInEditMode]
public class WaterShaderApplicator : MonoBehaviour {

    public Material effectMaterial;

    [Range(0,100)]
    public float redFalloff = 1;
    [Range (0, 100)]
    public float greenFalloff = 1;
    [Range (0, 100)]
    public float blueFalloff = 1;

    private Camera cam;
    private Light[] lights;

    void Awake () {
        cam = GetComponent<Camera> ();
    }

    void OnEnable () {
        if (cam == null)
            cam = GetComponent<Camera> ();

        lights = FindObjectsOfType<Light> ();

        cam.depthTextureMode |= DepthTextureMode.Depth;
    }
    
    void LateUpdate()
    {

        //Set params here
        var p = GL.GetGPUProjectionMatrix (cam.projectionMatrix, false);// Unity flips its 'Y' vector depending on if its in VR, Editor view or game view etc... (facepalm)
        p[2, 3] = p[3, 2] = 0.0f;
        p[3, 3] = 1.0f;
        var clipToWorld = Matrix4x4.Inverse (p * cam.worldToCameraMatrix) * Matrix4x4.TRS (new Vector3 (0, 0, -p[2, 2]), Quaternion.identity, Vector3.one);

        Vector4[] lightPositions = new Vector4[8];
        Vector4[] lightDirections = new Vector4[8];
        Vector4[] lightColors = new Vector4[8];
        Vector4[] lightProps = new Vector4[8];

        for (int i = 0; i < 8; i++) {
            lightColors[i] = lightPositions[i] = lightDirections[i] = lightProps[i] = Vector4.zero;

            if (lights.Length > i && lights[i].enabled) {
                lightPositions[i] = lights[i].transform.position;
                lightDirections[i] = lights[i].transform.forward;
                lightColors[i] = lights[i].color * lights[i].intensity;
                lightColors[i].w = lights[i].color.a;
                
                lightProps[i].w = 1;
                lightProps[i].z = lights[i].bounceIntensity;
                
                if (lights[i].type == LightType.Directional) {
                    lightPositions[i].w = 0;
                } else {
                    lightPositions[i].w = 1;
                    lightProps[i].x = lights[i].range;
                }
                
                if (lights[i].type == LightType.Spot){
                    lightProps[i].y = Mathf.Cos(Mathf.Deg2Rad * lights[i].spotAngle / 2);
                }
            }
        }

        Shader.SetGlobalVectorArray ("_LightPositions", lightPositions);
        Shader.SetGlobalVectorArray ("_LightDirections", lightDirections);
        Shader.SetGlobalVectorArray ("_LightColors", lightColors);
        Shader.SetGlobalVectorArray ("_LightProps", lightProps);
        Shader.SetGlobalInt("_NumPoints", 20);
    }

}