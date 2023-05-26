using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    static SoundPlayer instance;
    AudioSource audioSource;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
        }
    }

    public static void PlaySound(AudioClip audioClip)
    {
        if(instance!= null)
        {
            instance.audioSource.PlayOneShot(audioClip, audioClip.length);
        }
    }
}
