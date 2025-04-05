#ifndef WATER_DENSITY_DEF
#define WATER_DENSITY_DEF

uniform float _RedTransmittance, _GreenTransmittance, _BlueTransmittance;
uniform float _DensityScale, _SunScale, _MaxDistance;

float3 GetDepthColor(float depth) {
    
    depth = max(depth, 0);
    float r = exp(-depth / _RedTransmittance);
    float g = exp(-depth / _GreenTransmittance);
    float b = exp(-depth / _BlueTransmittance);
    return float3(r, g, b);
}

float3 GetPositionColor (float3 worldPos){
    return GetDepthColor(worldPos.y);
}

void GetPositionColor_float(float3 worldPos, out float3 color){
    color = GetPositionColor(worldPos);
}

void GetDepthColor_float(float distance, out float3 color){
    color = GetDepthColor(distance);
}

#endif