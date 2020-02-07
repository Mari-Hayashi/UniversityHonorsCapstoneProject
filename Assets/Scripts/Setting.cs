using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public enum Music {no_music = 0, binaural_beat, mozart }
public enum TaskType { Attention = 0, Memorization, Calculation }
[System.Serializable]
public class Setting : Singleton<Setting>
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

    [SerializeField]
    private GameObject loading;

    public static Session session;
    public static string playerName;

    public static int currentTask = 0;

    AudioSource audioSource;

    public const int eachTaskLength = 10; // minutes

    public static string fileName()
    {
        string musicString;
        int taskNumber = currentTask - 1; // (0, 1, 2)
        if (session == null || session.tasks.Count < 1) return "test";
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

        Debug.Log(Application.persistentDataPath);
        return Application.persistentDataPath + "/" + playerName + "-" + taskName + "-" + musicString  + "-" + taskNumber.ToString() + ".csv";
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        session = new Session();
        loading.SetActive(false);
    }

    public void MusicButtonPressed(int typeInt)
    {
        audioSource.Play();
        Music type = (Music)typeInt;
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
        loading.SetActive(true);
        audioSource.Play();
        SceneManager.LoadScene("Name");
    }

    public static void goToNextScene()
    {
        if (currentTask < 3)
        {
            SceneManager.LoadScene(session.getSceneName(currentTask));
            currentTask++;
        }
        else // Session done.
        {
            SceneManager.LoadScene("SessionDone");
        }
    }

    public static Music getCurrentMusic()
    {
        return session.tasks[currentTask - 1].music;
    }
}

public class Session
{
    public List<Task> tasks;
    public int currentTaskIndex;
    public int currentMusicIndex;

    public Session()
    {
        tasks = new List<Task>();
        currentTaskIndex = 0;
        currentMusicIndex = 0;
    }

    public void Clear()
    {
        tasks.Clear();
        currentTaskIndex = 0;
        currentMusicIndex = 0;
    }

    public void addTaskType(TaskType type)
    {
        if (currentTaskIndex == 3) return;

        if (tasks.Count <= currentTaskIndex)
        {
            Task newTask = new Task();
            tasks.Add(newTask);
        }

        tasks[currentTaskIndex].taskType = type;
        currentTaskIndex++;
    }

    public void addMusicType(Music type)
    {
        if (currentMusicIndex == 3) return;

        if (tasks.Count <= currentMusicIndex)
        {
            Task newTask = new Task();
            tasks.Add(newTask);
        }
        
        tasks[currentMusicIndex].music = type;
        currentMusicIndex++;
    }

    public string getSceneName(int taskNumber)
    {
        TaskType type = tasks[taskNumber].taskType;

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
