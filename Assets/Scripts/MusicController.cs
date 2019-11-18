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
        audioSource = GetComponent<AudioSource>();
        switch (Setting.music)
        {
            case Music.silend_music:
                audioSource.clip = loud_music;
                audioSource.Play();
                break;
            case Music.loud_music:
                audioSource.clip = silent_music;
                audioSource.Play();
                break;
        }
    }
}
