using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public enum Music {no_music = 0, binaural_beat, mozart }
public enum TaskType { Attention, Memorization, Calculation }
[System.Serializable]
public class Setting : MonoBehaviour
{
    [SerializeField]
    private Text no_music_button;
    private const string no_music_text = "No Music";
    [SerializeField]
    private Text binaural_beat_button;
    private const string binaural_beat_text = "Binaural";
    [SerializeField]
    private Text mozart_button;
    private const string mozart_text = "Mozart";

    [SerializeField]
    private Text attention_button;
    private const string attention_text = "Attention";
    [SerializeField]
    private Text memorization_button;
    private const string memorization_text = "Memorization";
    [SerializeField]
    private Text calculation_button;
    private const string calculation_text = "Calculation";

    public static Session session;


    AudioSource audioSource;

    public static string fileName(string playerName, int taskNumber)
    {
        string musicString;
        switch (session.tasks[taskNumber].music)
        {
            case Music.no_music:
                musicString = "NoMusic";
                break;
            case Music.binaural_beat:
                musicString = "BinauralBeat";
                break;
            case Music.mozart:
                musicString = "Mozart";
                break;
            default:
                musicString = "MusicUnspecified";
                break;
        }
        string taskName = session.getSceneName(taskNumber);

        string date = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();

        Debug.Log(Application.persistentDataPath);
        return Application.persistentDataPath + "/" + playerName + "-" + taskName + "-" + musicString  + "-" + date + ".csv";
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        session = new Session();
    }

    public void MusicButtonPressed(Music type)
    {
        audioSource.Play();
        session.addMusicType(type);

        switch (type)
        {
            case Music.no_music:
                no_music_button.text = no_music_text + " (" + session.currentMusicIndex + ")";
                break;
            case Music.binaural_beat:
                binaural_beat_button.text = binaural_beat_text + " (" + session.currentMusicIndex + ")";
                break;
            case Music.mozart:
                mozart_button.text = mozart_text + " (" + session.currentMusicIndex + ")";
                break;
        }
    }

    public void TaskButtonPressed(int type)
    {
        audioSource.Play();
        session.addTaskType((TaskType)type);

        switch ((TaskType)type)
        {
            case TaskType.Attention:
                attention_button.text = attention_text + " (" + session.currentTaskIndex + ")";
                break;
            case TaskType.Calculation:
                calculation_button.text = calculation_text + " (" + session.currentTaskIndex + ")";
                break;
            case TaskType.Memorization:
                memorization_button.text = memorization_text + " (" + session.currentTaskIndex + ")";
                break;
        }
    }

    public void ResetButtonPressed()
    {
        audioSource.Play();
        no_music_button.text = no_music_text;
        binaural_beat_button.text = binaural_beat_text;
        mozart_button.text = mozart_text;
        attention_button.text = attention_text;
        calculation_button.text = calculation_text;
        memorization_button.text = memorization_text;
        session.Clear();
    }

    public void OKButtonPressed()
    {
        audioSource.Play();
        // Go to the first session.
    }

}

public class Session
{
    public List<Task> tasks;
    public int currentTaskIndex;
    public int currentMusicIndex;

    public Session()
    {
        Task emptyTask = new Task();
        tasks = new List<Task>();
        tasks.Add(emptyTask);
        tasks.Add(emptyTask);
        tasks.Add(emptyTask);
        currentTaskIndex = 0;
        currentMusicIndex = 0;
    }

    public void Clear()
    {
        tasks.Clear();
        Task emptyTask = new Task();
        tasks = new List<Task>();
        tasks.Add(emptyTask);
        tasks.Add(emptyTask);
        tasks.Add(emptyTask);
        currentTaskIndex = 0;
        currentMusicIndex = 0;
    }

    public void addTaskType(TaskType type)
    {
        if (currentTaskIndex == 3) return;

        tasks[currentTaskIndex].taskType = type;
        currentTaskIndex++;
    }

    public void addMusicType(Music type)
    {
        if (currentTaskIndex == 3) return;

        tasks[currentMusicIndex].music = type;
        currentMusicIndex++;
    }

    public string getSceneName(int taskNumber)
    {
        TaskType type = tasks[taskNumber + 1].taskType;

        switch (type)
        {
            case TaskType.Attention:
                return "Attention";
            case TaskType.Calculation:
                return "Calculation";
            case TaskType.Memorization:
                return "Memorization";
        }

        return "";
    }
}

public class Task
{
    public TaskType taskType;
    public Music music;
}
