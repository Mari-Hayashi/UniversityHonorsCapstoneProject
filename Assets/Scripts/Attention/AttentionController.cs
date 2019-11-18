using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;

[System.Serializable]
public class AttentionController : Singleton<AttentionController>
{
    [SerializeField]
    private GameObject GameDoneTxt;

    public const int numImages = 7;
    private TextAsset csvFile;
    private const string CSVPath = "AttentionProblemSets";
    private List<AttentionQuestion> questionList;
    private const float Duration = 5f; // seconds
    private int sessionLength; // minutes
    private const float disabledDuration = 0.5f; // seconds.
    private static int curQuestionIndex;

    private ImageSetter imageSetter;
    private float timeSessionBegin;
    private float timeQuestionBegin;

    private AudioSource audioSource;

    public static bool buttonEnabled;

    private static int sessionNum;
    public static string playerName;
    public static bool gameGoingOn;

    [SerializeField]
    private AudioClip correct;
    [SerializeField]
    private AudioClip wrong;

    private const string DescriptionString =
    "Date(MM/DD/YY)," +
    "Time," +
    "Section Number," +
    "Time From Beginning," +
    "First image," +
    "First responce time," +
    "Second image," +
    "Second responce time," +
    "Third image," +
    "Third responce time," +
    "Fourth image," +
    "Fourth responce time," +
    "Fifth image," +
    "Fifth responce time," +
    "Sixth image," +
    "Sixth responce time," +
    "Seventh image," +
    "Seventh responce time";
    private const string FruitDescriptionString =
        "00: Red Apple," +
        "01: Green Apple," +
        "10: Orange," +
        "11: Lemon," +
        "last digit:," +
        "0: Right angle," +
        "1: Upside down";


    private StreamWriter streamwriter;
    FileInfo fileinfo;

    private const string COMMA = ",";

    public void init()
    {
        sessionLength = Setting.session_length;
        audioSource = GetComponent<AudioSource>();
        fileinfo = new FileInfo(Setting.fileName(playerName, "Attention"));
        streamwriter = fileinfo.AppendText();
        streamwriter.WriteLine(DescriptionString);
        streamwriter.WriteLine(FruitDescriptionString);

        imageSetter = gameObject.GetComponent<ImageSetter>();
        timeSessionBegin = Time.realtimeSinceStartup;
        sessionNum = 0;
        gameGoingOn = true;
        startSession();
        StartCoroutine(GameBody());
    }

    private void displayQuestion()
    {
        if (imageSetter == null) Debug.Log("HITOSHI");
        imageSetter.SetImages(questionList[curQuestionIndex].ImageString);
    }
    private void shuffleQuestions()
    {
        for (int n = questionList.Count - 1; n >= 1; --n)
        {
            int k = UnityEngine.Random.Range(0, n + 1);
            AttentionQuestion value = questionList[k];
            questionList[k] = questionList[n];
            questionList[n] = value;
        }
    }
    private void readCSV()
    {
        questionList = new List<AttentionQuestion>();
        csvFile = new TextAsset();
        csvFile = Resources.Load<TextAsset>(CSVPath);

        StringReader reader = new StringReader(csvFile.text);
        string line = reader.ReadLine();
        while (line != null)
        {
            string[] valuesInString = line.Split(',');
            string[] imageValues = new string[numImages];
            for (int i = 0; i < numImages; ++i)
            {
                imageValues[i] = valuesInString[i];
            }
            questionList.Add(new AttentionQuestion(imageValues));
            line = reader.ReadLine();
        }
        reader.Close();
    }
    IEnumerator GameBody()
    {
        while (true)
        {
            timeQuestionBegin = Time.realtimeSinceStartup;
            buttonEnabled = false;
            displayQuestion();
            yield return new WaitForSeconds(disabledDuration);
            buttonEnabled = true;
            yield return new WaitForSeconds(Duration - disabledDuration);
            writeCSV();
            curQuestionIndex++;
            if (Time.realtimeSinceStartup - timeSessionBegin > sessionLength * 60)
            {
                done();
                break;
            }
            if (curQuestionIndex >= questionList.Count)
            {
                startSession();
            }
        }
    }
    private void startSession()
    {
        sessionNum++;
        Debug.Log("Session " + sessionNum.ToString());
        readCSV();
        shuffleQuestions();
        curQuestionIndex = 0;
    }
    public void buttonPressed(int i)
    {
        questionList[curQuestionIndex].ResponceTime[i] = (Time.realtimeSinceStartup - timeQuestionBegin);
        if (questionList[curQuestionIndex].ImageString[i] == "000" || questionList[curQuestionIndex].ImageString[i] == "101")
            audioSource.PlayOneShot(correct);
        else audioSource.PlayOneShot(wrong);
    }
    private void done()
    {
        streamwriter.Flush();
        streamwriter.Close();
        gameGoingOn = false;
        GameDoneTxt.SetActive(true);
    }
    private void writeCSV()
    {
        float timeSinceBeginningOfSession = Time.realtimeSinceStartup - timeSessionBegin;
        streamwriter.WriteLine
        (
            DateTime.Now.ToString("MM/dd/yyyy") + COMMA +
            DateTime.Now.ToString("hh:mm:ss") + COMMA +
            sessionNum.ToString() + COMMA +
            timeSinceBeginningOfSession + COMMA +
            questionList[curQuestionIndex].responceTimeCSV()
         );
    }

    private new void OnApplicationQuit()
    {
        Debug.Log("Application is terminated.");
        done();
    }

    private void OnApplicationPause()
    {
        Debug.Log("Application is terminated.");
        done();
    }

}