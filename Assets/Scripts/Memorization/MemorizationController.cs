using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MemorizationController : Singleton<MemorizationController>
{
    [SerializeField]
    private Image spriteDisplay;
    [SerializeField]
    private GameObject spriteDisplayObj;
    [SerializeField]
    private Image[] choiceButtons;
    [SerializeField]
    private TextMesh questionText;
    [SerializeField]
    private TextMesh indexText;
    [SerializeField]
    private GameObject showingScreen;
    [SerializeField]
    private GameObject answeringScreen_buttons;
    [SerializeField]
    private GameObject answeringScreen_texts;

    private TextAsset csvFile;
    private const string SpriteInfoCSVPath = "MSTResources/";
    private const string SessionInfoCSVFileName = "ProblemSets";
    public const string SpritePathFormat = "MSTResources/Set {0}/";
    public string SpritePath;

    private const string questionString = "Which one is the {0} picture?";
    private const string endString = "Your session is done!";

    private List<MemorizationQuestion> QuestionList;
    private MemorizationQuestion CurrentQuestion;

    private int sessionLength;
    private float timeAtBeginningOfSession;
    private float timeAtBeginningOfQuestion;

    private int[] numImages;
    // Store number of images for each difficulty. 
    // numImages[4] stores number of images with difficulty of 4.

    private int[] 
        ImagesWithDifficulty1, 
        ImagesWithDifficulty2, 
        ImagesWithDifficulty3,
        ImagesWithDifficulty4, 
        ImagesWithDifficulty5;

    private int curQuestionIndex;
    private const int numQuestionPerSession = 30;
    public const int numChoices = 4;
    public const float displayInterval = 2f;

    private StreamWriter streamwriter;
    FileInfo fileinfo;

    private bool end;
    AudioSource audioSource;

    public static string playerName = "Hitoshi3";
    private const string DescriptionString = 
        "Date(MM/DD/YY)," +
        "Time," +
        "Section Number," +
        "Image Set Used," +
        "Time From Beginning," +
        "Responce Time," +
        "Contain similar picture in choices?(Y/N)," +
        "Difficulty," +
        "Choice1," +
        "Choice2," +
        "Choice3," +
        "Choice4," +
        "Answer," +
        "Choosen Answer," +
        "Pictures";
    private int sessionNumber;
    public const string COMMA = ",";

    private static readonly string[] imageSets = {"1", "2", "3", "4", "5", "6", "C", "D", "E", "F"};
    private string[] shuffledImageSets;
    private const int numImageSet = 10;

    public void init()
    {
        sessionLength = Setting.eachTaskLength;
        numImages = new int[6];
        for (int i = 0; i < 6; ++i) numImages[i] = 0;

        sessionNumber = 1;
        shuffledImageSets = StringShuffle(imageSets, 10);
        ReadSpriteInfo(shuffledImageSets[sessionNumber - 1]);
        SpritePath = string.Format(SpritePathFormat, shuffledImageSets[sessionNumber]);

        Shuffle(ImagesWithDifficulty1, numImages[1]);
        Shuffle(ImagesWithDifficulty2, numImages[2]);
        Shuffle(ImagesWithDifficulty3, numImages[3]);
        Shuffle(ImagesWithDifficulty4, numImages[4]);
        Shuffle(ImagesWithDifficulty5, numImages[5]);

        QuestionList = new List<MemorizationQuestion>();
        PopulateQuestionList();
        ShuffleQuestionList();
        Debug.Log(QuestionList[1].choicesString());

        fileinfo = new FileInfo(Setting.fileName()); // fix
        streamwriter = fileinfo.AppendText();
        streamwriter.WriteLine(DescriptionString);

        curQuestionIndex = 0;
        end = false;
        timeAtBeginningOfSession = Time.realtimeSinceStartup;
        DisplayQuestion();
        audioSource = GetComponent<AudioSource>();

        MusicController.instance.musicStart();
    }

    private void DisplayQuestion()
    {
        AnsweringScreenSetActive(false);
        CurrentQuestion = QuestionList[curQuestionIndex];
        StartCoroutine("ShowImages");
    }
    IEnumerator ShowImages()
    {
        for (int curIndex = 0; curIndex < CurrentQuestion.numSprites; ++curIndex) {
            indexText.text = (curIndex + 1).ToString() + " / " + CurrentQuestion.numSprites.ToString();
            spriteDisplay.overrideSprite = CurrentQuestion.getDisplaySprite(curIndex);
            yield return new WaitForSeconds(displayInterval);
        }
        DisplayChoices();
    }
    private void DisplayChoices()
    {
        questionText.text = string.Format(questionString, CurrentQuestion.indexString());
        for (int i = 0; i < numChoices; ++i)
        {
            choiceButtons[i].overrideSprite = CurrentQuestion.getChoiceSprite(i);
        }
        AnsweringScreenSetActive(true);
    }
    private void AnsweringScreenSetActive(bool b)
    {
        timeAtBeginningOfQuestion = Time.realtimeSinceStartup;
        answeringScreen_buttons.SetActive(b);
        answeringScreen_texts.SetActive(b);
        showingScreen.SetActive(!b);
        spriteDisplayObj.SetActive(!b);
    }
    private void PopulateQuestionList()
    {
        csvFile = new TextAsset();
        csvFile = Resources.Load<TextAsset>(SessionInfoCSVFileName);

        StringReader reader = new StringReader(csvFile.text);
        string line = reader.ReadLine();
        while (line != null)
        {
            string[] valuesInString = line.Split(',');
            int difficulty = int.Parse(valuesInString[4]);
            int numImages = int.Parse(valuesInString[0]);

            string[] imageFiles = new string[numImages];

            string[] choices = new string[numChoices];
            int choiceIndex = 0;

            string[] notAnswers;
            if (difficulty == 0) notAnswers = new string[numImages - 1];
            else notAnswers = new string[numImages - 2];
            int notAnswerIndex = 0;

            bool answerUsed = false;
            int curIndex = 1; // Index in valuesInString
            string answerFileName = "";

            // Populate imageFiles
            for (int imageFilesIndex = 0; imageFilesIndex < numImages; ++imageFilesIndex)
            {
                if (valuesInString[curIndex] == "" || curIndex > 3)
                {
                    string filename = returnPlaceHolder();
                    imageFiles[imageFilesIndex] = filename;

                    if (difficulty == 0 && !answerUsed)
                    {
                        choices[choiceIndex] = filename;
                        answerFileName = filename;
                        choiceIndex++;
                        answerUsed = true;
                    }
                    else
                    {
                        notAnswers[notAnswerIndex] = filename;
                        notAnswerIndex++;
                    }
                }
                else
                {
                    ImagePair tempImagePair = returnImagePair(int.Parse(valuesInString[curIndex]));
                    imageFiles[imageFilesIndex] = tempImagePair.image1;
                    imageFilesIndex++;
                    imageFiles[imageFilesIndex] = tempImagePair.image2;

                    if (difficulty == int.Parse(valuesInString[curIndex]) && !answerUsed)
                    {
                        choices[choiceIndex] = tempImagePair.image1;
                        choices[choiceIndex + 1] = tempImagePair.image2;
                        answerFileName = tempImagePair.image1;
                        choiceIndex += 2;
                        answerUsed = true;
                    }
                    else
                    {
                        notAnswers[notAnswerIndex] = tempImagePair.image1;
                        notAnswers[notAnswerIndex + 1] = tempImagePair.image2;
                        notAnswerIndex += 2;
                    }
                }
                curIndex += 1;
            }

           notAnswers = StringShuffle(notAnswers, notAnswerIndex);

            for (int i = choiceIndex; i < numChoices; ++i) choices[i] = notAnswers[i - choiceIndex];

            choices = StringShuffle(choices, numChoices);
            imageFiles = StringShuffle(imageFiles, numImages);
            QuestionList.Add(new MemorizationQuestion(imageFiles, choices, answerFileName, numImages, difficulty));
            line = reader.ReadLine();
        }

        reader.Close();
    }
    private string returnPlaceHolder()
    {
        int max = 1;
        int filenum;
        for (int i = 2; i < 6; ++i)
        {
            if (numImages[max] < numImages[i]) max = i;
        }

        switch (max)
        {
            case 1:
                filenum = ImagesWithDifficulty1[numImages[1] - 1];
                break;
            case 2:
                filenum = ImagesWithDifficulty2[numImages[2] - 1];
                break;
            case 3:
                filenum = ImagesWithDifficulty3[numImages[3] - 1];
                break;
            case 4:
                filenum = ImagesWithDifficulty4[numImages[4] - 1];
                break;
            case 5:
                filenum = ImagesWithDifficulty5[numImages[5] - 1];
                break;
            default:
                filenum = 0;
                break;
        }

        numImages[max]--;
        return filenum.ToString("000") + "a";
    }
    private ImagePair returnImagePair(int difficulty)
    {
        int filenum;

        switch (difficulty)
        {
            case 1:
                filenum = ImagesWithDifficulty1[numImages[difficulty] - 1];
                break;
            case 2:
                filenum = ImagesWithDifficulty2[numImages[difficulty] - 1];
                break;
            case 3:
                filenum = ImagesWithDifficulty3[numImages[difficulty] - 1];
                break;
            case 4:
                filenum = ImagesWithDifficulty4[numImages[difficulty] - 1];
                break;
            case 5:
                filenum = ImagesWithDifficulty5[numImages[difficulty] - 1];
                break;
            default:
                filenum = 0;
                break;
        }

        numImages[difficulty]--;
        return new ImagePair(filenum);
    }
    public void choiceButtonPressed(int loc)
    {
        audioSource.Play();
        if (end) SceneManager.LoadScene("Title");
        QuestionList[curQuestionIndex].gotAnswer(loc);
        writeOnLog();
        if (Time.realtimeSinceStartup - timeAtBeginningOfSession >= sessionLength * 60)
        {
            done();
            return;
        }
        curQuestionIndex++;
        if (curQuestionIndex >= numQuestionPerSession)
        {
            loadSession();
            return;
        }
        DisplayQuestion();
    }
    public void Shuffle(int[] array, int size)
    {
        for (int n = size - 1; n >= 1; --n)
        {
            int k = UnityEngine.Random.Range(0, n + 1);
            int value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
    }
    public string[] StringShuffle(string[] array, int size)
    {
        for (int n = size - 1; n >= 1; --n)
        {
            int k = UnityEngine.Random.Range(0, n + 1);
            string value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
        return array;
    }
    private void ShuffleQuestionList()
    {
        for (int n = QuestionList.Count - 1; n >= 1; --n)
        {
            int k = UnityEngine.Random.Range(0, n + 1);
            MemorizationQuestion value = QuestionList[k];
            QuestionList[k] = QuestionList[n];
            QuestionList[n] = value;
        }
    }
    private void ReadSpriteInfo(string setName)
    {
        csvFile = new TextAsset();
        csvFile = Resources.Load<TextAsset>(SpriteInfoCSVPath + "Set" + setName + " bins");

        StringReader reader = new StringReader(csvFile.text);
        string line = reader.ReadLine();
        while (line != null)
        {
            string[] valuesInString = line.Split('\t');
            numImages[int.Parse(valuesInString[1])] += 1;
            line = reader.ReadLine();
        }
        reader.Close();

        ImagesWithDifficulty1 = new int[numImages[1]];
        ImagesWithDifficulty2 = new int[numImages[2]];
        ImagesWithDifficulty3 = new int[numImages[3]];
        ImagesWithDifficulty4 = new int[numImages[4]];
        ImagesWithDifficulty5 = new int[numImages[5]];

        int curIndex1 = 0, curIndex2 = 0, curIndex3 = 0, curIndex4 = 0, curIndex5 = 0;

        reader = new StringReader(csvFile.text);
        line = reader.ReadLine();
        while (line != null)
        {
            string[] valuesInString = line.Split('\t');
            switch (int.Parse(valuesInString[1]))
            {
                case 1:
                    ImagesWithDifficulty1[curIndex1] = int.Parse(valuesInString[0]);
                    curIndex1++;
                    break;
                case 2:
                    ImagesWithDifficulty2[curIndex2] = int.Parse(valuesInString[0]);
                    curIndex2++;
                    break;
                case 3:
                    ImagesWithDifficulty3[curIndex3] = int.Parse(valuesInString[0]);
                    curIndex3++;
                    break;
                case 4:
                    ImagesWithDifficulty4[curIndex4] = int.Parse(valuesInString[0]);
                    curIndex4++;
                    break;
                case 5:
                    ImagesWithDifficulty5[curIndex5] = int.Parse(valuesInString[0]);
                    curIndex5++;
                    break;
            }
            line = reader.ReadLine();
        }
        reader.Close();
        Debug.Log("CSV read done.");
    }

    private void done()
    {
        if (streamwriter != null)
        {
            streamwriter.Flush();
            streamwriter.Close();
        }
        end = true;
        Setting.goToNextScene();
    }

    private void writeOnLog()
    {
        float timeSinceBeginning = Time.realtimeSinceStartup - timeAtBeginningOfSession;
        float timeTaken = Time.realtimeSinceStartup - timeAtBeginningOfQuestion;
        char similarPicture = CurrentQuestion.difficulty == 0 ? 'N' : 'Y';
        
        streamwriter.WriteLine
        (
            DateTime.Now.ToString("MM/dd/yyyy") + COMMA +
            DateTime.Now.ToString("hh:mm:ss") + COMMA +
            sessionNumber.ToString() + COMMA +
            shuffledImageSets[sessionNumber].ToString() + COMMA +
            timeSinceBeginning.ToString() + COMMA +
            timeTaken.ToString() + COMMA +
            similarPicture + COMMA +
            CurrentQuestion.difficulty.ToString() + COMMA +
            CurrentQuestion.choicesString() +
            CurrentQuestion.sprites[CurrentQuestion.answerIndexInSprites] + COMMA +
            CurrentQuestion.answerSelected + COMMA +
            CurrentQuestion.spritesString()
         );
    }

    private void loadSession()
    {
        Debug.Log("Go to next session");
        sessionNumber++;
        SpritePath = string.Format(SpritePathFormat, shuffledImageSets[sessionNumber]);

        for (int i = 0; i < 6; ++i) numImages[i] = 0;
        ReadSpriteInfo(shuffledImageSets[sessionNumber - 1]);
    
        Shuffle(ImagesWithDifficulty1, numImages[1]);
        Shuffle(ImagesWithDifficulty2, numImages[2]);
        Shuffle(ImagesWithDifficulty3, numImages[3]);
        Shuffle(ImagesWithDifficulty4, numImages[4]);
        Shuffle(ImagesWithDifficulty5, numImages[5]);

        QuestionList.Clear();
        QuestionList = new List<MemorizationQuestion>();
        PopulateQuestionList();
        ShuffleQuestionList();

        curQuestionIndex = 0;
        DisplayQuestion();
    }

    private new void OnApplicationQuit()
    {
        done();
    }

    private void OnApplicationPause()
    {
        done();
    }
}

public class ImagePair
{
    public string image1;
    public string image2;
    public ImagePair(int ID)
    {
        image1 = ID.ToString("000") + 'a';
        image2 = ID.ToString("000") + 'b';
    }
}
