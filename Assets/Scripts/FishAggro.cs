using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAggro : MonoBehaviour
{

    public float timeToAnger = 3, timeToForget = 10, timeToCalm = 5;
    public Animator animator;
    
    float angerAmount;

    Dictionary<LightSource, float> lightAggroTable = new Dictionary<LightSource, float>();

    HashSet<LightSource> currentShiningLights = new HashSet<LightSource>();
    
    public void ClearAggros(){
        lightAggroTable.Clear();
        currentShiningLights.Clear();
        attackTargets.Clear();
        angerAmount = 0;
    }

    public void AddLight(LightSource light)
    {
        if (!lightAggroTable.ContainsKey(light)) lightAggroTable[light] = 0;
        if (!currentShiningLights.Contains(light)) currentShiningLights.Add(light);

        RefreshStartled();
    }

    public void RemoveLight(LightSource light)
    {
        if (currentShiningLights.Contains(light)) currentShiningLights.Remove(light);

        RefreshStartled();
    }

    void RefreshStartled()
    {
        animator.SetBool("Startled", lightAggroTable.Count > 0);
        animator.SetBool("Attack", attackTargets.Count > 0);
    }

    public LightSource GetStartleTarget()
    {
        LightSource target = null;
        float maxAggro = float.MinValue;
        foreach (var kv in lightAggroTable)
        {
            LightSource light = kv.Key;
            float aggro = kv.Value;

            if (aggro > maxAggro)
            {
                maxAggro = aggro;
                target = light;
            }
        }

        return target;
    }

    public LightSource GetAttackTarget()
    {
        LightSource target = null;
        
        float proximity = float.MaxValue;

        foreach (LightSource light in attackTargets){
            if (!light.isOn) continue;
            
            float dist = Vector3.Distance(light.lastVisiblePosition, transform.position);
            
            if (dist < proximity){
                proximity = dist;
                target = light;
            }
        }

        return target;

    }

    void FixedUpdate()
    {
        foreach (LightSource light in new List<LightSource>(lightAggroTable.Keys))
        {
            bool isShining = light.isOn && currentShiningLights.Contains(light);
            float aggro = lightAggroTable[light];

            if (isShining)
            {
                aggro += Time.fixedDeltaTime / timeToAnger;

                if (aggro > 1)
                {
                    aggro = 1;
                    AttackTarget(light);
                }
            }
            else
            {
                aggro -= Time.fixedDeltaTime / timeToForget;
                if (aggro < 0)
                {
                    lightAggroTable.Remove(light);
                    continue;
                }
            }

            lightAggroTable[light] = aggro;
        }
        
        if (angerAmount > 0){
            foreach (LightSource light in lightAggroTable.Keys){
                AttackTarget(light);
            }
            
            if (attackTargets.Count == 0){
                angerAmount -= Time.deltaTime;
            }
        }
        
        
        RefreshStartled();
    }

    HashSet<LightSource> attackTargets = new HashSet<LightSource>();
    void AttackTarget(LightSource light)
    {
        if (!attackTargets.Contains(light)){
            attackTargets.Add(light);
        }
        angerAmount = 1;
    }

}
