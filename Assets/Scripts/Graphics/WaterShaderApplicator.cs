using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Rendering.Experimental.LookDev;
using UnityEngine;

[ExecuteInEditMode]
public class WaterShaderApplicator : MonoBehaviour
{

    private Light[] lights;

    void OnEnable()
    {
        lights = FindObjectsOfType<Light>();
    }

    void LateUpdate()
    {

        Vector4[] lightPositions = new Vector4[8];
        Vector4[] lightDirections = new Vector4[8];
        Vector4[] lightColors = new Vector4[8];
        Vector4[] lightProps = new Vector4[8];

        for (int i = 0; i < 8; i++)
        {
            lightColors[i] = lightPositions[i] = lightDirections[i] = lightProps[i] = Vector4.zero;

            if (lights.Length > i && lights[i].enabled)
            {
                lightPositions[i] = lights[i].transform.position;
                lightDirections[i] = lights[i].transform.forward;
                lightColors[i] = lights[i].color * lights[i].intensity;
                lightColors[i].w = lights[i].color.a;

                lightProps[i].w = 1;
                lightProps[i].z = lights[i].bounceIntensity;

                if (lights[i].type == LightType.Directional)
                {
                    lightPositions[i].w = 0;
                }
                else
                {
                    lightPositions[i].w = 1;
                    lightProps[i].x = lights[i].range;
                }

                if (lights[i].type == LightType.Spot)
                {
                    lightProps[i].y = Mathf.Cos(Mathf.Deg2Rad * lights[i].spotAngle / 2);
                }
            }
        }

        Shader.SetGlobalVectorArray("_LightPositions", lightPositions);
        Shader.SetGlobalVectorArray("_LightDirections", lightDirections);
        Shader.SetGlobalVectorArray("_LightColors", lightColors);
        Shader.SetGlobalVectorArray("_LightProps", lightProps);
        Shader.SetGlobalInt("_NumPoints", 20);
    }

}