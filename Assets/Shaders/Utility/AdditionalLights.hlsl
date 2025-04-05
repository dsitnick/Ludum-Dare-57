float3 intersectCone(float3 rayOrigin, float3 rayDirection, float3 coneOrigin, float3 coneDirection, float coneAngle, float radius, out float t1, out float t2)
{
    float cosa = sin(coneAngle/2);
    float3 co = rayOrigin - coneOrigin;

    float a = dot(rayDirection,coneDirection)*dot(rayDirection,coneDirection) - cosa*cosa;
    float b = 2. * (dot(rayDirection,coneDirection)*dot(co,coneDirection) - dot(rayDirection,co)*cosa*cosa);
    float c = dot(co,coneDirection)*dot(co,coneDirection) - dot(co,co)*cosa*cosa;

    float det = b*b - 4.*a*c;
    if (det < 0.) return float3(0,0,0);

    det = sqrt(det);
    t1 = (-b - det) / (2. * a);
    t2 = (-b + det) / (2. * a);
    
    float3 result = float3 (0,0,1);
    
    float tMin = min(t1, t2);
    float tMax = max(t1, t2);
    
    if (tMin >= 0){
        result.x = 1;
    }
    if (tMax >= 0){
        result.y = 1;
    }
    result.z = 1;
    return result;

    /*// This is a bit messy; there ought to be a more elegant solution.
    float t = t1;
    if (t < 0. || t2 > 0. && t2 < t) t = t2;
    if (t < 0.) return noHit;

    float3 cp = rayOrigin + t*rayDirection - coneOrigin;
    float h = dot(cp, coneDirection);
    if (h < 0. || h > radius) return noHit;

    float3 n = normalize(cp * dot(coneDirection, cp) / dot(cp, cp) - coneDirection);

    return Hit(t, n, s.m);*/
}

#include "../WaterDensity.hlsl"
#include "../DepthLighting.hlsl"

float3 GetAdditionalLightColors(float3 WorldPos, float3 WorldNormal){
    float3 LightColor = 0;
    for (int i = 0; i < 8; i++)
    {
        if (_LightPositions[i].w > 0)
        {
            //Point or spotlight
            float3 lightPos = _LightPositions[i].xyz;
            float3 diff = lightPos.xyz - WorldPos;
            float distance = length(diff);
            float3 direction = diff / distance;
            
            //Spotlight
            if (_LightProps[i].y > 0){
                float angleCos = -dot(_LightDirections[i], direction);
                
                if (abs(acos(_LightProps[i].y)) > angleCos){
                    continue;
                }
                
            }
            float normalFalloff = saturate(dot(WorldNormal, direction)) * 0.5 + 0.5;
            float distanceFalloff = exp(-distance * 2 / _LightProps[i].x);
            float3 colorFalloff = GetDepthColor(distance);
            colorFalloff *= distanceFalloff;
            //colorFalloff = min(colorFalloff, distanceFalloff);
            
            LightColor += _LightColors[i] * colorFalloff * normalFalloff;
        }
        else
        {
            //if (_LightDirections[i].y >= 0) break;
            
            float depth = (WorldPos.y - _LightPositions[i].y) / _LightDirections[i].y;
            float3 depthColor = GetDepthColor(depth / 4);
            
            LightColor += _LightColors[i] * depthColor * (saturate(dot(WorldNormal, _LightDirections[i])) * 0.5 + 0.5);
        }
    }
    
    return LightColor;
}
 
void GetAdditionalLightColors_float(float3 WorldPos, float3 WorldNormal, out float3 LightColor)
{
    LightColor = GetAllLightsAtPoint(WorldPos, WorldNormal); //GetAdditionalLightColors(WorldPos, WorldNormal);
}

void GetSkyboxLightColor_float(float3 CameraPos, float3 LookDirection, out float3 LightColor){
    LightColor = SkyboxRaycast(CameraPos, LookDirection);
}