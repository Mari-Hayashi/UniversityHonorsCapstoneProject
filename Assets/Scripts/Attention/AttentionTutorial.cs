using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AttentionTutorial : MonoBehaviour
{
    private enum currentScreen { Title = 0, ExplainApple, ExplainOrder, PracticeApple, ResultApple, Explainorange, PracticeOrange, ResultOrange, Countdown };

    [SerializeField]
    private TextMesh explanationTxt;
    [SerializeField]
    private GameObject explanationScreen;

    [SerializeField]
    private GameObject sampleApple;
    [SerializeField]
    private GameObject sampleOrange;

    [SerializeField]
    private GameObject[] buttons; // 2
    private Text[] buttonTexts;
    [SerializeField]
    private GameObject mainGameObjects;
    [SerializeField]
    private GameObject fruits;
    [SerializeField]
    private GameObject practiceGameObject_SelectAns;
    [SerializeField]
    private GameObject practiceGameObject_Display;
    [SerializeField]
    private GameObject choiceButtons;

    [SerializeField]
    private AudioClip boo;
    // Start is called before the first frame update
    private AudioSource audioSource;
    private currentScreen currentscreen;

    private const string TitleStr = "ATTENTION";
    private const string ExplainAppleStr = "Tap Only Red and\nRight Angle\nApples.";
    private const string ExplainTapOrderStr = "You can only\ntap from top\nto bottom.";
    private const string ExplainOrangeStr = "Next, Tap Only\nUpside down\nOranges.";
    private const string NameEmptyString = "Name cannot be empty.\nType your name.";
    private const string GoodJobApple = "Good Job!";
    private const string GoodJobOrange = "Good Job!\nNow, we will\nget started.";

    private static readonly string[] ApplePractice = { "000", "000", "010", "000", "010", "001", "000" };
    private static readonly string[] OrangePractice = { "101", "100", "101", "111", "110", "100", "101" };

    private ImageSetter imageSetter;

    void Start()
    {
        currentscreen = currentScreen.Title;
        mainGameObjects.SetActive(false);
        fruits.SetActive(false);
        audioSource = this.GetComponent<AudioSource>();
        imageSetter = this.GetComponent<ImageSetter>();

        buttonTexts = new Text[2];
        for (int i = 0; i < 2; ++i) buttonTexts[i] = buttons[i].GetComponentInChildren<Text>();
    }

    private void changeTexts()
    {
        switch (currentscreen)
        {
            case currentScreen.ExplainApple:
                explanationTxt.text = ExplainAppleStr;
                buttonTexts[1].text = "Next";
                break;
            case currentScreen.ExplainOrder:
                explanationTxt.text = ExplainTapOrderStr;
                buttonTexts[1].text = "Practice";
                curButton = -1;
                break;
            case currentScreen.ResultApple:
                explanationTxt.text = GoodJobApple;
                buttonTexts[1].text = "OK";
                break;
            case currentScreen.Explainorange:
                explanationTxt.text = ExplainOrangeStr;
                buttonTexts[1].text = "Practice";
                curButton = -1;
                break;
            case currentScreen.ResultOrange:
                explanationTxt.text = GoodJobOrange;
                buttonTexts[1].text = "OK";
                break;
        }
    }

    public void topButton()
    {
        StartGame();
    }

    public void StartGame()
    {
        currentscreen = currentScreen.Countdown;
        buttons[0].SetActive(false);
        buttons[1].SetActive(false);
        StartCoroutine("CountDown");
    }
    public void bottomButton()
    {
        audioSource.Play();
        switch (currentscreen)
        {
            case currentScreen.Title:
                audioSource.Play();
                currentscreen = currentScreen.ExplainApple;
                buttons[0].SetActive(false);
                sampleApple.SetActive(true);
                changeTexts();
                break;
            case currentScreen.ExplainApple:
                currentscreen = currentScreen.ExplainOrder;
                changeTexts();
                break;
            case currentScreen.ExplainOrder:
                currentscreen = currentScreen.PracticeApple;
                setPracticeScreenActive(true);
                break;
            case currentScreen.ResultApple:
                currentscreen = currentScreen.Explainorange;
                sampleOrange.SetActive(true);
                changeTexts();
                break;
            case currentScreen.Explainorange:
                currentscreen = currentScreen.PracticeOrange;
                setPracticeScreenActive(true);
                break;
            case currentScreen.ResultOrange:
                StartGame();
                break;
            default:
                break;
        }
    }

    IEnumerator CountDown()
    {
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
        AttentionController.instance.init();
        this.gameObject.SetActive(false);
    }

    int curButton = -1;
    public void buttonPressed(int i)
    {
        if (i < curButton)
        {
            audioSource.PlayOneShot(boo);
            return;
        }
        curButton = i;

        string[] fruits;
        if (currentscreen == currentScreen.PracticeApple) fruits = ApplePractice;
        else fruits = OrangePractice;

        if (fruits[i] == "000" || fruits[i] == "101")
        {
            imageSetter.displayCircle(i);
            audioSource.Play();
            if (i == 6)
            {
                StartCoroutine("PracticeEnd");
            }
        }
        else
        {
            audioSource.PlayOneShot(boo);
        }
    }

    IEnumerator PracticeEnd()
    {
        yield return new WaitForSeconds(1f);
        if (currentscreen == currentScreen.PracticeApple) currentscreen = currentScreen.ResultApple;
        else currentscreen = currentScreen.ResultOrange;
        setPracticeScreenActive(false);
        imageSetter.displayCircle(-1);
        changeTexts();
    }

    private void setPracticeScreenActive(bool b)
    {
        fruits.SetActive(b);
        explanationScreen.SetActive(!b);
        explanationTxt.gameObject.SetActive(!b);
        buttons[1].SetActive(!b);

        if (b)
        {
            if (currentscreen == currentScreen.PracticeApple)
            {
                imageSetter.SetImages(ApplePractice);
                sampleApple.SetActive(false);
            }
            else
            {
                imageSetter.SetImages(OrangePractice);
                sampleOrange.SetActive(false);
            }
        }
    }
}
