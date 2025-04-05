#ifndef DEPTH_LIGHTING_DEF
#define DEPTH_LIGHTING_DEF

#include "WaterDensity.hlsl"

uniform float4 _LightPositions[8];
uniform float4 _LightDirections[8];
uniform float4 _LightColors[8];
uniform float4 _LightProps[8];

uniform int _NumPoints;

struct Ray {
    float3 origin;
    float3 direction;
    float startT, endT;
};

/*bool CheckSphereIntersection(Ray ray, int lightIndex, out float t, out bool isInsideSphere)
{
    float3 rayOrigin = ray.origin;
    float3 rayDirection = ray.direction;
    float3 sphereOrigin = _LightPositions[lightIndex].xyz;
    float sphereRadius = _LightProps[lightIndex].x;
    
	float3 op = sphereOrigin - rayOrigin;
    
    isInsideSphere = dot(op, op) > sphereRadius * sphereRadius;
    
    float b = dot(op, rayDirection);
    float det = b * b - dot(op, op) + sphereRadius * sphereRadius;
    if (det < 0.) return false;

    det = sqrt(det);
    t = b - det;
    if (t < 0.) t = b + det;
    if (t < 0.) return false;

    return true;
}*/

float ProjectOntoRay(float3 position, float3 rayOrigin, float3 rayDirection){
    return dot(rayDirection, position - rayOrigin);
}

bool CheckSphereIntersection(Ray ray, int lightIndex, out float tMin, out float tMax, out bool isInsideSphere)
{
    float3 rayOrigin = ray.origin;
    float3 rayDirection = ray.direction;
    float3 sphereOrigin = _LightPositions[lightIndex].xyz;
    float sphereRadius = 12;//_LightProps[lightIndex].x;
    
	float3 op = sphereOrigin - rayOrigin;
    
    isInsideSphere = dot(op, op) < sphereRadius * sphereRadius;
    
    float b = dot(op, rayDirection);
    float det = b * b - dot(op, op) + sphereRadius * sphereRadius;
    if (det < 0.) return isInsideSphere;

    det = sqrt(det);
    
    float t1 = b - det;
    float t2 = b + det;
    
    tMin = min(t1, t2);
    tMax = max(t1, t2);

    return true;
}

void CheckInsideCone(Ray ray, int lightIndex, out bool isInsideCone, out bool isInsideConeShadoww){
    float3 rayOrigin = ray.origin;
    float3 rayDirection = ray.direction;
    float3 coneOrigin = _LightPositions[lightIndex].xyz;
    float3 coneDirection = normalize(_LightDirections[lightIndex].xyz);
    float cosa = _LightProps[lightIndex].y;
    
    float originDot = dot(normalize(rayOrigin - coneOrigin), coneDirection);
    
    isInsideCone = (originDot >= 0 && cosa < originDot);
    isInsideConeShadoww = (-originDot >= 0 && cosa < -originDot);
}

bool CheckConeIntersection(Ray ray, int lightIndex, out bool isInsideCone, out float tMin, out float tMax)
{
    float3 rayOrigin = ray.origin;
    float3 rayDirection = ray.direction;
    float3 coneOrigin = _LightPositions[lightIndex].xyz;
    float3 coneDirection = normalize(_LightDirections[lightIndex].xyz);
    float coneRadius = _LightProps[lightIndex].x;
    float cosa = _LightProps[lightIndex].y;
    
    float originDot = dot(normalize(rayOrigin - coneOrigin), coneDirection);
    
    isInsideCone = (originDot >= 0 && cosa < originDot);
    
    float3 co = rayOrigin - coneOrigin;

    float a = dot(rayDirection,coneDirection)*dot(rayDirection,coneDirection) - cosa*cosa;
    float b = 2. * (dot(rayDirection,coneDirection)*dot(co,coneDirection) - dot(rayDirection,co)*cosa*cosa);
    float c = dot(co,coneDirection)*dot(co,coneDirection) - dot(co,co)*cosa*cosa;

    float det = b*b - 4.*a*c;
    if (det < 0.) return false;

    det = sqrt(det);
    float t1 = (-b - det) / (2. * a);
    float t2 = (-b + det) / (2. * a);
    
    tMin = min(t1, t2);
    tMax = max(t1, t2);
    
    float t = t1;
    if (t < 0. || t2 > 0. && t2 < t) t = t2;
    
    float3 cp = rayOrigin + t*rayDirection - coneOrigin;
    float h = dot(cp, coneDirection);
    if (h < 0.) return false;
    
    /*float sphereMin, sphereMax;
    bool isInsideSphere;
    if (CheckSphereIntersection(ray, lightIndex, sphereMin, sphereMax, isInsideSphere) && sphereMin > 0){
        
        if (tMin > 0){
            //Positive cone case, replace tMax with min(tMax, and sphereMax), ignore tMin
            tMax = min(tMax, sphereMax);
        }else if (tMax > 0){
            //Split cone case, replace tMax with min(tMax and sphereMax)
            tMax = min(tMax, sphereMax);
        }
        
    }else{
        //return false;
    }*/
    
    return true;
}

float3 GetLightColorAtPoint(int lightIndex, float3 worldPos){
    float3 lightColor = _LightColors[lightIndex].rgb;
    
    if (_LightPositions[lightIndex].w > 0) {
        //Point or spotlight
        float3 lightPos = _LightPositions[lightIndex].xyz;
        float3 diff = lightPos.xyz - worldPos;
        float distance = length(diff);
        float3 direction = diff / distance;
        
        float distanceFalloff = distance * 10 / _LightColors[lightIndex].a / _LightProps[lightIndex].x + 0.5;
        distanceFalloff = 1 / (distanceFalloff * distanceFalloff);
            
        float fadeScale = 1;
        //Spotlight
        if (_LightProps[lightIndex].y > 0){
            float angleCos = -dot(_LightDirections[lightIndex].xyz, direction);
            
            angleCos = lerp(angleCos, 1, distanceFalloff);
            
            if (_LightProps[lightIndex].y >= angleCos){
                //return 0;
                fadeScale = 0;
            }else{
                fadeScale = saturate( (angleCos - _LightProps[lightIndex].y) / 0.2 );
            }
            
        }
        
        //fadeScale = 1;
            
        float3 colorFalloff = GetDepthColor(distance);
        colorFalloff *= distanceFalloff * fadeScale;
        
        return lightColor * colorFalloff;
    } else {
        float depth = (worldPos.y - _LightPositions[lightIndex].y) / _LightDirections[lightIndex].y;
        float3 depthColor = GetDepthColor(depth / 2 / _SunScale);
        
        return lightColor * depthColor * _LightColors[lightIndex].a;
    }
}

float3 GetLightColorOnSurface(int lightIndex, float3 worldPos, float3 worldNormal){
    float3 lightColor = _LightColors[lightIndex].rgb;
    
    if (_LightPositions[lightIndex].w > 0) {
        //Point or spotlight
        float3 lightPos = _LightPositions[lightIndex].xyz;
        float3 diff = lightPos.xyz - worldPos;
        float distance = length(diff);
        float3 direction = diff / distance;
        
        float distanceFalloff = distance * 2 / _LightProps[lightIndex].x + 1;
        distanceFalloff = 1 / (distanceFalloff * distanceFalloff);
        
        float fadeScale = 1;
        //Spotlight
        if (_LightProps[lightIndex].y > 0){
            float angleCos = -dot(_LightDirections[lightIndex].xyz, direction);
            
            if (_LightProps[lightIndex].y >= angleCos){
                return 0;
            }else{
                fadeScale = saturate( (angleCos - _LightProps[lightIndex].y)  );
            }
            
        }
        
        float normalFalloff = saturate(dot(worldNormal, direction)) * 0.5 + 0.5;
        float3 colorFalloff = GetDepthColor(distance);
        colorFalloff *= distanceFalloff;
        
        fadeScale = max(0, fadeScale) / 2;
        
        return lightColor * colorFalloff * normalFalloff * fadeScale;
    } else {
        float depth = (worldPos.y - _LightPositions[lightIndex].y) / _LightDirections[lightIndex].y;
        float3 depthColor = GetDepthColor(depth / 2 / _SunScale);
        
        return lightColor * depthColor * (saturate(dot(worldNormal, _LightDirections[lightIndex])) * 0.5 + 0.5);
    }
}

float3 GetAllLightsAtPoint(float3 worldPos, float3 worldNormal){
    float3 result = 0;
    for (int i = 0; i < 8; i++){
        result += GetLightColorOnSurface(i, worldPos, worldNormal);
    }
    return result;
}

//Given a transmittance ray, returns the color of the light as it passes through
float3 SpotlightRaycast(Ray ray, int lightIndex, float rayDistance, bool insideCone, bool isReverse){
    
    float3 resultLight = 0;
    //float3 samplePos = ray.origin + ray.direction * ray.startT;
    float rayLength = abs(ray.startT - ray.endT);
    float totalDistance = 0;
    
    float stepSize = 1;
    
    int numPoints = floor(rayLength / stepSize) + 1;
    numPoints = min(numPoints, _NumPoints);
    
    //isReverse = true;
    
    for (int i = 0; i < numPoints; i++){
        
        float t;
        if (!isReverse){
            t = ray.startT + totalDistance;
        }else{
            t = ray.endT - totalDistance;
        }
        
        float3 lightAtPoint = GetLightColorAtPoint(lightIndex, ray.origin + ray.direction * t);
        float3 transmittance = GetDepthColor(t);
        
        //transmittance = 1;
        
        float lightScale = stepSize;
        
        //Do the remainder pass, scaling by the leftover distance
        //Unscaled if there is less than 1 step
        if (i == numPoints - 1 && i > 0){
            lightScale = rayLength - totalDistance;
        }
        
        float3 light = lightAtPoint * transmittance * lightScale;
        
        //if (i > 5) light *= 0;
        
        resultLight += light;
        //samplePos += ray.direction * stepSize;
        totalDistance += stepSize;
    }
    
    return resultLight;
}

//Given a transmittance ray, returns the color of the light as it passes through
float3 TransmittanceRaycast(Ray ray, int lightIndex, float rayDistance){
    
    float3 resultLight = 0;
    float3 samplePos = ray.origin + ray.direction * ray.startT;
    float rayLength = abs(ray.startT - ray.endT);
    
    float stepSize = rayLength / (_NumPoints - 1);
    
    for (int i = 0; i < _NumPoints; i++){
        //if (i < 5 || i >= numPoints - 5) continue;
        
        float3 lightAtPoint = GetLightColorAtPoint(lightIndex, samplePos);
        float3 transmittance = GetDepthColor(ray.startT + i * stepSize);
        
        float3 light = lightAtPoint * transmittance * stepSize;
        
        resultLight += light;
        samplePos += ray.direction * stepSize;
    }
    
    return resultLight;
}

float3 GetRayColorFromLight(Ray ray, int lightIndex, float rayDistance){
    if (_LightProps[lightIndex].w == 0){
        return 0;
    }
    //Basic case for point/directional lights
    if (_LightProps[lightIndex].y <= 0){
        return TransmittanceRaycast(ray, lightIndex, rayDistance);
    }
    
    //Special case for spotlights, calculate cone intersection points
    
    float tMin, tMax, sMin, sMax;
    bool isInsideCone, isInsideSphere, isInsideConeShadow;
    
    float startT = ray.startT, endT = ray.endT;
    float shadowOffset = 0;
    float3 originalOrigin = ray.origin;
    
    CheckInsideCone(ray, lightIndex, isInsideCone, isInsideConeShadow);
    
    if (isInsideConeShadow){
        
        shadowOffset = ProjectOntoRay(_LightPositions[lightIndex].xyz, ray.origin, ray.direction);
        
        ray.origin += normalize(ray.direction) * shadowOffset;
        
    }
    
    if (!CheckConeIntersection(ray, lightIndex, isInsideCone, tMin, tMax)){
        return 0;
    }
    
    ray.origin = originalOrigin;
    
    float tDistance = abs(tMax - tMin);
    if (!CheckSphereIntersection(ray, lightIndex, sMin, sMax, isInsideSphere)){
        //return 0;
        sMin = -1;
    }
    if (tMin > 0) {
        if (isInsideCone){
            //If inside the cone, and max/min are positive, then the ray should start at 0, and end at min
            endT = tMin;
        }else{
            //If outside the cone, and max/min are both positive, then the ray should cast between the two
            startT = tMin;
            endT = tMax;
        }
    } else if (tMax < 0) {
        if (isInsideCone){
            //If inside the cone, and max/min are both negative, then the ray should cast normal
        }else{
            //If outside the cone max/min are both negative, the ray can be ignored
            return 0;
        }
    } else {
        if (isInsideCone){
            //If inside the cone, and max/min have split signs, then the ray should start at 0, and end at max
            endT = tMax;
        }else{
            //If outside the cone, and max/min have split signs, then the ray should start at max, and end at _MaxDistance - max
            startT = tMax + shadowOffset;
            //endT += tMax;
            //endT = min(endT, tMax);
        }
    }
    
    //return abs(endT - startT);
    
    if (ray.endT < startT){
        return 0;
    }
    
    //return ray.endT - ray.startT;
    ray.startT = startT;// min(ray.startT, startT);
    ray.endT = min(ray.endT, endT);
    
    if (isInsideSphere && sMax > 0){
        //ray.endT = min(ray.endT, sMax);
    }else if (sMin > 0) {
        //ray.startT = max(ray.startT, sMin);
    }
    
    bool isReverse = false;
    if (isInsideCone){
        isReverse = dot(_LightDirections[lightIndex].xyz, ray.direction) < 0;
        //if (isReverse) return 1;
        return lerp(
            SpotlightRaycast(ray, lightIndex, rayDistance, isInsideCone, true),
            SpotlightRaycast(ray, lightIndex, rayDistance, isInsideCone, false),
            dot(_LightDirections[lightIndex].xyz, ray.direction) * 0.5 + 0.5
        );
    }else{
        return SpotlightRaycast(ray, lightIndex, rayDistance, isInsideCone, false);
    }
}


float4 SphereDebug(float3 rayOrigin, float3 rayDirection, float rayDistance){
    Ray ray = {rayOrigin, normalize(rayDirection), 0, rayDistance};
    float tMin, tMax;
    bool inSphere;
    
    if (CheckSphereIntersection(ray, 1, tMin, tMax, inSphere)){
        //if (inSphere) return float4(0,1,0,1);
        float4 result = (0,0,0,1);
        if (tMin >= 0){
            if (tMax >= 0){
                return float4(1,0,0,1);
            }else{
                return float4(0,1,0,1);
            }
        }else{
            if (tMax >= 0){
                return float4(0,0,1,1);
            }else{
                return float4(.2,.2,0.3,1);
            }
        }
        
        if (tMin >= 0){
            result.x = 1;
        }
        if (tMax >= 0){
            result.y = 1;
        }
        if (inSphere) return float4(1,0,0,1);
    }else{
        return 0;
    }
}

float4 ConeDebug(float3 rayOrigin, float3 rayDirection, float rayDistance){
    Ray ray = {rayOrigin, normalize(rayDirection), 0, rayDistance};
    float tMin, tMax;
    bool inCone, inConeShadow;
    CheckInsideCone(ray, 1, inCone, inConeShadow);
    if (CheckConeIntersection(ray, 1, inCone, tMin, tMax)){
        if (inCone) return float4(0,1,0,1);
        float4 result = (0,0,0,1);
        if (tMin >= 0){
            if (tMax >= 0){
                return float4(1,0,0,1);
            }else{
                return float4(0,1,0,1);
            }
        }else{
            if (tMax >= 0){
                return float4(0,0,1,1);
            }else{
                return float4(.5,.5,0.5,1);
            }
        }
        
        if (tMin >= 0){
            result.x = 1;
        }
        if (tMax >= 0){
            result.y = 1;
        }
        if (inCone) return float4(1,0,0,1);
        return tMax / 1000;
    }else{
        return 0;
    }
}

float3 DepthLightingRaycast(float3 rayOrigin, float3 rayDirection, float rayDistance){
    float3 resultColor = 0;
    Ray ray = {rayOrigin, normalize(rayDirection), 0, rayDistance};
    
    for (int i = 0; i < 8; i++){
        resultColor += GetRayColorFromLight(ray, i, rayDistance);
    }
    
    return resultColor;
}

float3 SkyboxRaycast(float3 rayOrigin, float3 rayDirection){
    float3 resultColor = 0;
    
    for (int i = 0; i < 8; i++){
        if (_LightProps[i].w <= 0 || _LightPositions[i].w > 0 || _LightColors[i].a <= 0) continue;
        float3 pos = rayOrigin + rayDirection * _MaxDistance;
        pos.y = min(pos.y, _LightPositions[i].y - 20);
        
        resultColor += GetLightColorAtPoint(i, pos) / _LightColors[i].a;
    }
    
    return resultColor;
    
}

#endif