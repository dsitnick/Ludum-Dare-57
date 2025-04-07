using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessModifier : MonoBehaviour
{

    public float minVignetteIntensity, maxVignetteIntensity;
    public float minExposure, maxExposure;
    public Color minColorFilter, maxColorFilter;
    public Volume volume;
    
    public Oxygen oxygen;
    
    public bool isFading;
    
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;

    [Range(0, 1)]
    public float damageSlider;
    
    [Range(0, 1)]
    public float oxygenSlider;
    
    
    [Range(0, 1)]
    public float fadeSlider;

    void Start()
    {
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out colorAdjustments);
    }

    float lastValue, lastFade;
    void Update()
    {
        oxygenSlider = oxygen.remainingO2;
        float value = 1 - (1 - damageSlider) * (oxygenSlider);
        if (value != lastValue)
        {
            vignette.intensity.value = Mathf.Lerp(minVignetteIntensity, maxVignetteIntensity, value);
            colorAdjustments.postExposure.value = Mathf.Lerp(minExposure, maxExposure, value);
            colorAdjustments.colorFilter.value = Color.Lerp(minColorFilter, maxColorFilter, value);
            lastValue = value;
        }
        
        if (isFading) colorAdjustments.postExposure.value = Mathf.Lerp(0, -20, fadeSlider);

    }

}
