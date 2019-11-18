using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public enum Music {no_music = 0, silend_music, loud_music }
[System.Serializable]
public class Setting : MonoBehaviour
{
    [SerializeField]
    private Text music_button_text;
    [SerializeField]
    private Text input_field_text;

    public static Music music = Music.no_music;
    public static int session_length = 30;

    AudioSource audioSource;

    public static string fileName(string playerName, string taskName)
    {
        string musicString;
        switch (music)
        {
            case Music.no_music:
                musicString = "NoMusic";
                break;
            case Music.silend_music:
                musicString = "SilentMusic";
                break;
            case Music.loud_music:
                musicString = "LoudMusic";
                break;
            default:
                musicString = "MusicUnspecified";
                break;
        }
        string date = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();

        Debug.Log(Application.persistentDataPath);
        return Application.persistentDataPath + "/" + playerName + "-" + taskName + "-" + musicString  + "-" + date + ".csv";
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void MusicButtonPressed()
    {
        audioSource.Play();
        if ((int)music < 2) music++;
        else music = 0;

        switch (music)
        {
            case Music.no_music:
                music_button_text.text = "No Music";
                break;
            case Music.silend_music:
                music_button_text.text = "Silent Music";
                break;
            case Music.loud_music:
                music_button_text.text = "Loud Music";
                break;
        }
    }

    public void OkButtonPressed()
    {  
        audioSource.Play();
        if (input_field_text.text == "")
        {
            session_length = 30;
            SceneManager.LoadScene("Title");
        }

        int number;
        if (!int.TryParse(input_field_text.text, out number) || number <= 0)
        {
            input_field_text.text = "";
        }
        else
        {
            session_length = number;
            SceneManager.LoadScene("Title");
        }
    }

}
