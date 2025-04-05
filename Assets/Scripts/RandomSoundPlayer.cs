using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSoundPlayer : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip[] clips;
    public float pitchSpread;

    public void PlayRandomSound()
    {
        Debug.Log("Played Sound");
        audioSource.pitch = Random.Range(1 - pitchSpread * 0.5f, 1 + pitchSpread * 0.5f);
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }

}
