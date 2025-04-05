using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/DeepSeaEffectComponent", typeof(UniversalRenderPipeline))]
public class DeepSeaEffectComponent : VolumeComponent, IPostProcessComponent
{
    
    public ClampedFloatParameter redTransmittance = new ClampedFloatParameter(6, 0.1f, 200, true);
    public ClampedFloatParameter greenTransmittance = new ClampedFloatParameter(12, 0.1f, 200, true);
    public ClampedFloatParameter blueTransmittance = new ClampedFloatParameter(24, 0.1f, 200, true);
    
    public ClampedFloatParameter sunScale = new ClampedFloatParameter(1, 0, 10, true);
    public ClampedFloatParameter densityScale = new ClampedFloatParameter(0, 0, 1, true);
    public ClampedFloatParameter maxDistance = new ClampedFloatParameter(100, 0, 1000, true);
    
    // Tells when our effect should be rendered
    public bool IsActive() => true;
   
   	// I have no idea what this does yet but I'll update the post once I find an usage
    public bool IsTileCompatible() => true;
}