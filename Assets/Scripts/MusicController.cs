using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : Singleton<MusicController>
{
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip Mozart;
    [SerializeField]
    private AudioClip Binaural;

    public void musicStart()
    {
        audioSource = GetComponent<AudioSource>();
        Music music = Setting.getCurrentMusic();
        switch (music)
        {
            case Music.mozart:
                audioSource.clip = Mozart;
                audioSource.Play();
                break;
            case Music.binaural_beat:
                audioSource.clip = Binaural;
                audioSource.Play();
                break;
        }
    }
}
