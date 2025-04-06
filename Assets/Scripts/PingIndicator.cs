using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingIndicator : MonoBehaviour
{
    
    public float farDistanceThreshold = 1000, closeDistanceThreshold = 10;
    
    public float closePingDelay, farPingDelay, closePitch = 1, farPitch = 1;
    
    public Vector3 targetPos;
    public float pitchSpread;
    
    public AudioSource audioSource;
    public AudioClip[] pingClips;
    
    private float proximity;
    
    void Start(){
        StartCoroutine(PingSound());
    }
    
    void FixedUpdate(){
        proximity = Mathf.InverseLerp(closeDistanceThreshold, farDistanceThreshold, Vector3.Distance(targetPos, transform.position));
        proximity = Mathf.Clamp01(proximity);
    }
    
    IEnumerator PingSound(){
        while (true){
            yield return new WaitForSeconds(Mathf.Lerp(closePingDelay, farPingDelay, proximity));
            float pitch = Mathf.Lerp(closePitch, farPitch, proximity);
            pitch += Random.Range(-0.5f, 0.5f) * pitchSpread;
            
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(pingClips[Random.Range(0, pingClips.Length)]);
        }
    }
    
}
