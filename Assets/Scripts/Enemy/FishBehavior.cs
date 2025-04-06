using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FishBehavior : MonoBehaviour
{

    public FishWandering wandering;
    public FishAggro aggro;
    public PlayerInfoScriptableObject playerInfo;

    public RandomSoundPlayer startleSound, angrySound, attackSound;

    public Animator animator;

    public float swimSpeed = 1;
    public float velocityLerp = 8, rotationLerp = 8, retreatDuration = 7;
    public bool isAttacking, isRetreating;
    
    public UnityEvent onHit;

    private Vector3 targetPos;

    private const float ATTACK_RANGE = 4;

    void FixedUpdate()
    {
        LightSource target = aggro.GetAttackTarget() ?? aggro.GetStartleTarget();

        if (target)
        {
            targetPos = target.lastVisiblePosition;
        }
        else
        {
            targetPos = wandering.targetPos;
        }
    }

    public void Hit()
    {
        animator.SetTrigger("Hit");
        aggro.ClearAggros();
        onHit.Invoke();
        StartCoroutine(Retreat());
    }

    Vector3 velocity;
    void Update()
    {
        Vector3 targetDirection = (targetPos - transform.position).normalized;

        if (isRetreating)
        {
            targetDirection *= -1;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), rotationLerp * Time.deltaTime);
        velocity = Vector3.Lerp(velocity, transform.forward * swimSpeed, velocityLerp * Time.deltaTime);
        transform.position += velocity * Time.deltaTime;
    }

    IEnumerator Retreat()
    {
        yield return new WaitForSeconds(retreatDuration);
        animator.SetTrigger("Reset");
        wandering.GetNewPoint();
    }

    public void PlayStartleSound() => startleSound.PlayRandomSound();
    public void PlayAngrySound() => angrySound.PlayRandomSound();
    public void PlayAttackSound() => attackSound.PlayRandomSound();


}
