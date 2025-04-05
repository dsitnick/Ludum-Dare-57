// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Density Effect"
{
   Properties
   {
      _MainTex ("Source", 2D) = "white" {}
      _NoiseTex ("Noise Tex", 2D) = "white" {}
      _DepthScale("Depth Scale", float) = 1
      _Color("Color", Color) = (1,1,1,1)
      _Power("Light Power", Range(0, 10)) = 1
      _KValue("K Value", Range(-10, 10)) = 1
      _RedFalloff("Red Falloff", Range(0, 100)) = 1
      _GreenFalloff("Green Falloff", Range(0, 100)) = 1
      _BlueFalloff("Blue Falloff", Range(0, 100)) = 1
      _NoiseAmount("Noise Amount", Range(0, 1)) = 0.1
      _NoiseSpeed("Noise Speed", Range(0, 100)) = 1
      _NoiseScale("Noise Scale", Range(0, 100)) = 1
      _Bias("Bias", Range(0, 1)) = 0
   }
   SubShader
   {
      Cull Off 
      ZWrite Off 
      ZTest Always

      Tags { "Queue"="Overlay"}

      Pass
      {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
            
        #include "UnityCG.cginc"
        #include "Utility/AdditionalLights.hlsl"
        #include "DepthLighting.hlsl"
        #include "Lighting.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            float3 viewVector : TEXCOORD1;
        };

        v2f vert(appdata i)
        {
            v2f o;

            o.vertex = UnityObjectToClipPos(i.vertex);
            o.uv = i.texcoord;

            float3 viewVector = mul(unity_CameraInvProjection, float4(i.texcoord * 2 - 1, 0, -1));
            o.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));

            return o;
        }
            
        sampler2D _MainTex, _NoiseTex;
        sampler2D _CameraDepthTexture;
        float4 _MainTex_ST;
        float _DepthScale;
        float4 _Color;

        float3 sampleColorAtPoint(float3 worldPos){
            /*return 0;
		    float seaDepth = max(-worldPos.y, 0) + 5;
            float4 sunColor = float4(GetDepthColor(seaDepth),1) * _LightColor0;

            float4 lightsColor = 0;

            return sunColor + float4(lightsColor);*/
            
            float3 result = GetAdditionalLightColors(worldPos, 1);
            
            return result;
		}

        float3 GetRayColor(float3 origin, float3 rayDir, float numPoints, float depth, float densityScale, float depthScale, float4 originalColor){
            float3 samplePoint = origin;
            float rayLength = min(depth, _MaxDistance);
            float stepSize = rayLength / (numPoints - 1);
            float3 light = originalColor;
            
            for (int i = 0; i < numPoints; i++){
                float3 lightAtPoint = GetAdditionalLightColors(samplePoint, 1) * densityScale;
                float3 transmittance = GetDepthColor(i * stepSize);
                
                light += lightAtPoint * transmittance * stepSize;
                samplePoint += rayDir * stepSize;
            }
            
            return light;
        }

        float4 frag(v2f i) : COLOR {
            float2 uv = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
            float4 color = tex2D(_MainTex, uv);
            /*float rawDepth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));
            float depth = LinearEyeDepth(rawDepth) * _ProjectionParams.w;*/
            float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv.xy);
            depth = LinearEyeDepth(depth);
            
            float3 rayOrigin = _WorldSpaceCameraPos;
            float3 rayDir = normalize(i.viewVector);
            float rayDistance = min(depth, _MaxDistance);

            float3 resultColor = DepthLightingRaycast(rayOrigin, rayDir, rayDistance) * _DensityScale;
            
            resultColor = max(0, resultColor);
            resultColor += color;
            //return SphereDebug(rayOrigin, rayDir, rayDistance);
            //return ConeDebug(rayOrigin, rayDir, rayDistance);
            //return float4(intersectCone(_WorldSpaceCameraPos, normalize(i.viewVector), float3(0,0,0), float3(0,0,1), 10, 5, t1, t2),1);
           /* return intersectCone(
                _WorldSpaceCameraPos,
                i.viewVector,
                float3(0,0,0),
                float3(0,1,0),
                15,
                5
            );*/

            return float4(resultColor, 1);
        }
        ENDCG
    }

   }
   Fallback Off
}