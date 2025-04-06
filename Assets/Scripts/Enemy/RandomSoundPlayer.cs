using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class RandomSoundPlayer : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip[] clips;
    public float pitchSpread;

    public void PlayRandomSound()
    {
        audioSource.pitch = Random.Range(1 - pitchSpread * 0.5f, 1 + pitchSpread * 0.5f);
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }

}
