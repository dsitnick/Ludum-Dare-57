using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Oxygen : MonoBehaviour
{

    public float remainingO2;

    public float scalePerHundred = 0.8f;

    public float timeToDeplete;

    public Image indicatorImage;
    public Graphic[] tintGraphics;
    public Graphic[] dangerGraphics;
    [Range(0, 1)]
    public float indicatorFull = 1;

    public PlayerInfoScriptableObject playerInfo;

    public UnityEvent onCriticalO2, onDeath;

    private Color indicatorColor;
    private float damageRateAccumulation = 1;

    void Start()
    {
        remainingO2 = 1;
        indicatorColor = indicatorImage.color;
    }
    
    private bool isActive = false;
    public void Begin(){
        isActive = true;
        remainingO2 = 1;
    }

    void FixedUpdate()
    {
        if (!isActive) return;
        
        float depth = -playerInfo.position.y;
        remainingO2 -= damageRateAccumulation * Time.fixedDeltaTime / timeToDeplete;
        Refresh();

        indicatorImage.fillAmount = remainingO2 * indicatorFull;

    }

    public void TakeSubmarineDamage()
    {
        remainingO2 -= 0.15f;
        damageRateAccumulation *= 1.3f;
        damageRateAccumulation = Mathf.Max(damageRateAccumulation, 3);
        Refresh();
    }

    public void TakePlayerDamage()
    {
        remainingO2 *= 0.3f;
        Refresh();
    }

    float lastO2;
    void Refresh()
    {
        if (lastO2 > 0.25f && remainingO2 <= 0.25f)
        {
            Color c = remainingO2 > 0.25f ? indicatorColor : new Color(1, 0, 0, indicatorColor.a);
            foreach (Graphic g in tintGraphics) g.color = c;
            foreach (Graphic g in dangerGraphics) g.enabled = remainingO2 < 0.25f;
            
            onCriticalO2.Invoke();
        }
        
        if (lastO2 > 0 && remainingO2 <= 0){
            onDeath.Invoke();
        }
        
        lastO2 = remainingO2;
    }

}
