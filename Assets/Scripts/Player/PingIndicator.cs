using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PingIndicator : MonoBehaviour
{

    public float farDistanceThreshold = 1000, closeDistanceThreshold = 10;

    public float closePingDelay, farPingDelay, closePitch = 1, farPitch = 1;

    public float heightDifferenceThreshold = 25f;

    public Vector3 targetPos;
    public float pitchSpread;

    public AudioSource audioSource;
    public AudioClip[] pingClips;

    public Graphic noSignalGraphic;

    public Graphic[] pingGraphics;
    
    public UnityEvent onPing;

    public Image pingBar;
    public Image heightIcon;
    public Sprite heightUp, heightDown, heightEven;

    float pingTimer, currentPingDuration;

    Color pingBarColor;
    void Start()
    {
        pingBarColor = pingBar.color;
    }

    void Update()
    {
        if (pingTimer <= 0)
        {
            var objective = PipeObjective.GetClosestObjective(transform.position);
            if (objective == null)
            {
                enabled = false;
            }

            targetPos = objective.position;
            float distance = Vector3.Distance(targetPos, transform.position);
            bool noSignal = distance > farDistanceThreshold;
            float proximity = Mathf.InverseLerp(closeDistanceThreshold, farDistanceThreshold, distance);


            proximity = Mathf.Clamp01(proximity);
            float heightDifference = targetPos.y - transform.position.y;

            float pitch = Mathf.Lerp(closePitch, farPitch, proximity);
            pitch += Random.Range(-0.5f, 0.5f) * pitchSpread;

            if (true)
            {
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(pingClips[Random.Range(0, pingClips.Length)]);
            }

            if (heightDifference > heightDifferenceThreshold)
            {
                heightIcon.sprite = heightUp;
            }
            else if (heightDifference < -heightDifferenceThreshold)
            {
                heightIcon.sprite = heightDown;
            }
            else
            {
                heightIcon.sprite = heightEven;
            }
            pingBar.fillAmount = proximity;
            pingBar.enabled = !noSignal;
            noSignalGraphic.enabled = noSignal;

            currentPingDuration = Mathf.Lerp(closePingDelay, farPingDelay, proximity);
            pingTimer = currentPingDuration;
            
            onPing.Invoke();
        }

        pingTimer -= Time.deltaTime;
        Color c = pingBarColor;
        c.a *= Mathf.Clamp01(pingTimer / currentPingDuration);

        foreach (Graphic g in pingGraphics) g.color = c;
    }

}
