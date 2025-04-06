using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleSoundPlayer : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip[] clips;

    public float minDelay = 7f, maxDelay = 10f;

    void Start()
    {
        StartCoroutine(PlaySoundsRoutine());
    }

    IEnumerator PlaySoundsRoutine()
    {
        while (true)
        {
            audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

        }
    }

}
