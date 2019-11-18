using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CalculationTutorial : MonoBehaviour
{
    private enum currentScreen {Title = 0, Explain, Practice1, Practice2, Practice3, Result, Countdown};
    [SerializeField]
    private TextMesh explanationTxt;
    [SerializeField]
    private TextMesh Calculationtxt;
    [SerializeField]
    private GameObject[] buttons;
    private Text[] buttonTexts;
    [SerializeField]
    private GameObject mainGameObjects;

    private const string TitleStr = "CALCULATION";
    private const string ExplainStr = "Answer calculations\nas quick and\nas accurate\nas possible.";
    private const string ResultStr1 = "You got ";
    private const string ResultStr2 = "/3 correct!\nNow, we will\nget started.";

    private currentScreen currentscreen;
    private Calculation[] practiceCalculations;
    private int currentIndex;

    private int numCorrect;

    private AudioSource audioSource;

    void Start()
    {
        currentscreen = currentScreen.Title;
        numCorrect = 0;

        practiceCalculations = new Calculation[3];
        practiceCalculations[0] = new Calculation(2, 5, Operation.add);
        practiceCalculations[1] = new Calculation(12, 4, Operation.subtract);
        practiceCalculations[2] = new Calculation(6, 3, Operation.muptiply);

        buttonTexts = new Text[6];
        for (int i = 0; i < 6; ++i)
        {
            buttonTexts[i] = buttons[i].GetComponentInChildren<Text>();
        }
        audioSource = GetComponent<AudioSource>();
    }
    private void setCalculation(int index)
    {
        Calculation currentCalculation = practiceCalculations[index];
        currentIndex = index;
        int answerLocation = UnityEngine.Random.Range(0, CalculationController.numChoices);
        for (int i = 0; i < CalculationController.numChoices; ++i)
        {
            if (answerLocation == i) buttonTexts[i + 2].text = currentCalculation.actualAnswer.ToString();
            else if (i < answerLocation) buttonTexts[i + 2].text = currentCalculation.choices[i].ToString();
            else buttonTexts[i + 2].text = currentCalculation.choices[i - 1].ToString();
        }
        Calculationtxt.text = currentCalculation.calculationString();
    }
    private void changeTexts()
    {
        switch (currentscreen)
        {
            case currentScreen.Explain:
                explanationTxt.text = ExplainStr;
                buttonTexts[1].text = "Practice";
                break;
            case currentScreen.Practice1:
                explanationTxt.text = "";
                for (int i = 0; i < 6; ++i)
                {
                    if (i < 2) buttons[i].SetActive(false);
                    else buttons[i].SetActive(true);
                }
                setCalculation(0);
                break;
            case currentScreen.Practice2:
                setCalculation(1);
                break;
            case currentScreen.Practice3:
                setCalculation(2);
                break;
            case currentScreen.Result:
                Calculationtxt.text = "";
                explanationTxt.text = ResultStr1 + numCorrect.ToString() + ResultStr2;
                for (int i = 1; i < 6; ++i)
                {
                    if (i < 2) buttons[i].SetActive(true);
                    else buttons[i].SetActive(false);
                }
                buttonTexts[1].text = "Start!";
                break;
            default: break;
        }
    }
    public void topButton()
    {
        StartCoroutine(GameBegin());
    }

    public void bottomButton()
    {
        audioSource.Play();
        switch (currentscreen) {
            case currentScreen.Title:
                audioSource.Play();
                buttons[0].SetActive(false);
                currentscreen = currentScreen.Explain;
                changeTexts();
                break;
            case currentScreen.Explain:
                currentscreen = currentScreen.Practice1;
                changeTexts();
                break;
            case currentScreen.Result:
                StartCoroutine(GameBegin());
                break;
            default: break;
        }
    }
    IEnumerator GameBegin()
    {
        currentscreen = currentScreen.Countdown;
        buttons[1].SetActive(false);
        buttons[0].SetActive(false);
        explanationTxt.text = 3.ToString();
        audioSource.Play();
        yield return new WaitForSeconds(1f);
        explanationTxt.text = 2.ToString();
        audioSource.Play();
        yield return new WaitForSeconds(1f);
        explanationTxt.text = 1.ToString();
        audioSource.Play();
        yield return new WaitForSeconds(1f);
        mainGameObjects.SetActive(true);
        CalculationController.instance.init();
        this.gameObject.SetActive(false);
    }
    public void answerButton(int location)
    {
        audioSource.Play();
        if (int.Parse(buttonTexts[location + 2].text) == practiceCalculations[currentIndex].actualAnswer) numCorrect++;
        currentscreen++;
        changeTexts();
    }
}
