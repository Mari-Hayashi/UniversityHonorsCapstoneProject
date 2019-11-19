using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CalculationController : Singleton<CalculationController>
{
    [SerializeField]
    private TextMesh functiontext;
    [SerializeField]
    private Text[] buttons;
    [SerializeField]
    private GameObject tutorialMenu;
    [SerializeField]
    private TextMesh englishText;

    float initialTime;
    float curTime;
    private float StartTime;
    private float TimeTaken;

    public static string playerName;

    List<Calculation> calculations;

    //Number per section
    private const int numAdditionWithCarry = 20;
    private const int numAdditionWithoutCarry = 20;
    private const int numSubtractionWithBorrow = 20;
    private const int numSubtractionWithoutBorrow = 20;
    private const int numMultiplication = 10;
    private const int numDivision = 10;

    private const int AddSubtRangeFloor = 11;
    private const int AddSubtRangeCeil = 244;
    private const int MultDivRangeFloor = 2;
    private const int MultDivRangeCeil = 9;

    private int SessionLength;

    private const string DescriptionString = "Date(MM/DD/YY),Time,SectionNumber,Time From Beginning,Responce Time,Operand 1,Operand 2,Operation,Correct Answer,Chosen Answer,Choice1,Choice2,Choice3,Choice4";
    private const string OperationDescription = "Operation:,0 add,1 subtract,2 multiply,3 divide";

    int curSection;
    int curCalculationIndex;
    public const int numChoices = 4;
    private const string COMMA = ",";

    Calculation currentCalculation;

    private StreamWriter streamwriter;
    FileInfo fileinfo;
    AudioSource audioSource;
    private bool gameGoingOn = false;

    public void init()
    {
        SessionLength = Setting.eachTaskLength;
        gameGoingOn = true;
        curSection = 0;
        initialTime = Time.realtimeSinceStartup;
        produceProblems();
        fileinfo = new FileInfo(Setting.fileName());// fix
        streamwriter = fileinfo.AppendText();
        streamwriter.WriteLine(DescriptionString);
        streamwriter.WriteLine(OperationDescription);
        audioSource = GetComponent<AudioSource>();
        MusicController.instance.musicStart();
    }

    private void produceProblems()
    {
        calculations = new List<Calculation>();
        curSection++;

        produceAdditions();
        produceSubtractions();
        produceMultiplications();
        produceDivisions();

        shuffleCalculationSet();
        curCalculationIndex = 0;
        DisplayCalculation();
    }

    private void setButtontext()
    {
        int answerLocation = UnityEngine.Random.Range(0, numChoices);
        for (int i = 0; i < numChoices; ++i)
        {
            if (answerLocation == i) buttons[i].text = currentCalculation.actualAnswer.ToString();
            else if (i < answerLocation) buttons[i].text = currentCalculation.choices[i].ToString();
            else buttons[i].text = currentCalculation.choices[i - 1].ToString();
        }
    }
    private void DisplayCalculation()
    {
        if (Time.realtimeSinceStartup - initialTime >= SessionLength * 60) done();
        else if (curCalculationIndex >= calculations.Count) produceProblems();
        else
        {
            currentCalculation = calculations[curCalculationIndex];
            functiontext.text = currentCalculation.calculationString();
            setButtontext();
            StartTime = Time.realtimeSinceStartup;
        }
    }
    public void choiceButtonPressed(int button)
    {
        if (!gameGoingOn) SceneManager.LoadScene("Title");

        audioSource.Play();
        TimeTaken = Time.realtimeSinceStartup - StartTime;
        curTime = Time.realtimeSinceStartup - initialTime;


        currentCalculation.answerByPlayer = int.Parse(buttons[button].text);
        writeOnLog();
        curCalculationIndex++;
        DisplayCalculation();
    }
    private void writeOnLog()
    {
        string choicestring = "";
        for (int i = 0; i < numChoices; ++i)
        {
            choicestring += buttons[i].text + COMMA;
        }
        streamwriter.WriteLine
        (
            DateTime.Now.ToString("MM/dd/yyyy") + COMMA +
            DateTime.Now.ToString("hh:mm:ss") + COMMA +
            curSection.ToString() + COMMA +
            curTime.ToString() + COMMA +
            TimeTaken.ToString() + COMMA +
            currentCalculation.resultString() + COMMA +
            choicestring
         );
             
    }
    private void done()
    {
        streamwriter.Flush();
        streamwriter.Close();
        functiontext.text = "";
        gameGoingOn = false;
        Setting.goToNextScene();
    }
    /// --------------------- HELPERS ----------------------- ///
    private void produceAdditions()
    {
        int numCarry = 0;
        int numNoCarry = 0;

        while (numCarry < numAdditionWithCarry || numNoCarry < numAdditionWithoutCarry)
        {
            int oper1 = UnityEngine.Random.Range(AddSubtRangeFloor, AddSubtRangeCeil + 1);
            int oper2 = UnityEngine.Random.Range(AddSubtRangeFloor, AddSubtRangeCeil);
            if (oper2 >= oper1) oper2++;

            if (haveCarry(oper1, oper2))
            {
                if (numCarry >= numAdditionWithCarry) continue;
                numCarry++;
                Calculation add = new Calculation(oper1, oper2, Operation.add);
                add.haveCarry = true;
                calculations.Add(add);
            }
            else
            {
                if (numNoCarry >= numAdditionWithoutCarry) continue;
                numNoCarry++;
                Calculation add = new Calculation(oper1, oper2, Operation.add);
                add.haveCarry = false;
                calculations.Add(add);
            }

        }
    }
    private void produceSubtractions()
    {
        int numBorrow = 0;
        int numNoBorrow = 0;

        while (numBorrow < numSubtractionWithBorrow || numNoBorrow < numSubtractionWithoutBorrow)
        {
            int oper1 = UnityEngine.Random.Range(AddSubtRangeFloor, AddSubtRangeCeil + 1);
            int oper2 = UnityEngine.Random.Range(AddSubtRangeFloor, AddSubtRangeCeil);
            if (oper2 >= oper1) oper2++;

            if (oper2 > oper1) // set smaller number to second operand
            {
                int temp = oper1;
                oper1 = oper2;
                oper2 = temp;
            }

            if (haveBorrow(oper1, oper2))
            {
                if (numBorrow >= numSubtractionWithBorrow) continue;
                numBorrow++;
                Calculation subt = new Calculation(oper1, oper2, Operation.subtract);
                subt.haveBorrow = true;
                calculations.Add(subt);
            }
            else
            {
                if (numNoBorrow >= numSubtractionWithoutBorrow) continue;
                numNoBorrow++;
                Calculation subt = new Calculation(oper1, oper2, Operation.subtract);
                subt.haveBorrow = false;
                calculations.Add(subt);
            }

        }
    }
    private void produceMultiplications()
    {
        for (int i = 0; i < numMultiplication; ++i)
        {
            int oper1 = UnityEngine.Random.Range(MultDivRangeFloor, MultDivRangeCeil + 1);
            int oper2 = UnityEngine.Random.Range(MultDivRangeFloor, MultDivRangeCeil);
            if (oper2 >= oper1) oper2++;

            Calculation mult = new Calculation(oper1, oper2, Operation.muptiply);
            calculations.Add(mult);
        }
    }
    private void produceDivisions()
    {
        for (int i = 0; i < numDivision; ++i)
        {
            int oper2 = UnityEngine.Random.Range(MultDivRangeFloor, MultDivRangeCeil + 1);
            int oper1 = UnityEngine.Random.Range(MultDivRangeFloor, MultDivRangeCeil);
            if (oper1 >= oper2) oper2++;
            oper1 *= oper2;

            Calculation div = new Calculation(oper1, oper2, Operation.divide);
            calculations.Add(div);
        }
    }
    private void shuffleCalculationSet()
    {
        for (int n = calculations.Count - 1; n >= 1; --n)
        {
            int k = UnityEngine.Random.Range(0, n + 1);
            Calculation value = calculations[k];
            calculations[k] = calculations[n];
            calculations[n] = value;
        }
    }
    private int firstDigit(int a)
    {
        return a % 10;
    }
    private int secondDigit(int a)
    {
        return (a % 100) - (a % 10);
    }
    private bool haveCarry(int a, int b)
    {
        if (firstDigit(a) + firstDigit(b) >= 10) return true;
        if (secondDigit(a) + secondDigit(b) >= 10) return true;

        return false;
    }
    private bool haveBorrow(int a, int b)
    {
        if (firstDigit(a) - firstDigit(b) < 0) return true;
        if (secondDigit(a) - secondDigit(b) < 0) return true;

        return false;
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
