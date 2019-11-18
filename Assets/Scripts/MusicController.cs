using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip loud_music;
    [SerializeField]
    private AudioClip silent_music;

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // TODO: Fix below
        /*
        switch (Setting.m)
        {
            case Music.binaural_beat:
                audioSource.clip = loud_music;
                audioSource.Play();
                break;
            case Music.mozart:
                audioSource.clip = silent_music;
                audioSource.Play();
                break;
        }
        */
    }
}
