using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MemorizationTutorial : MonoBehaviour
{
    private enum currentScreen { Title = 0, Name, Explain, Practice, Result, Countdown };
    [SerializeField]
    private TextMesh explanationTxt;
    [SerializeField]
    private TextMesh IndexText;
    [SerializeField]
    private SpriteRenderer displaySprite;
    [SerializeField]
    private GameObject inputField;
    [SerializeField]
    private Text inputText;
    [SerializeField]
    private GameObject[] buttons; // 2
    private Text[] buttonTexts;
    [SerializeField]
    private GameObject mainGameObjects;
    [SerializeField]
    private GameObject tutorialObjects;
    [SerializeField]
    private GameObject practiceGameObject_SelectAns;
    [SerializeField]
    private GameObject practiceGameObject_Display;
    [SerializeField]
    private GameObject choiceButtons;

    private const string TitleStr = "MEMORIZATION";
    private const string TypeNameStr = "Please type\nyour name";
    private const string ExplainStr = "Memorize images\nand ordering.";
    private const string ResultCorrect = "You got it correct!\nNow, we will\nget started.";
    private const string ResultWrong = "You got it wrong!\nLet's try again.";
    private static readonly string[] imageFiles = {"001a", "002a", "003a", "004a"};

    private currentScreen currentscreen;
    private int currentIndex;
    private MemorizationQuestion[] questions;

    private int numCorrect;

    private AudioSource audioSource;
    private bool practiceWasCorrect;

    void Start()
    {
        currentscreen = currentScreen.Title;
        inputField.SetActive(false);
        mainGameObjects.SetActive(false);

        buttonTexts = new Text[2];
        for (int i = 0; i < 2; ++i) buttonTexts[i] = buttons[i].GetComponentInChildren<Text>();
        audioSource = GetComponent<AudioSource>();
    }

    private void changeTexts()
    {
        switch (currentscreen)
        {
            case currentScreen.Name:
                explanationTxt.text = TypeNameStr;
                buttonTexts[1].text = "OK";
                break;
            case currentScreen.Explain:
                explanationTxt.text = ExplainStr;
                buttonTexts[1].text = "Practice";
                break;
            case currentScreen.Result:
                if (practiceWasCorrect)
                {
                    explanationTxt.text = ResultCorrect;
                    buttonTexts[1].text = "Start!";
                }
                else
                {
                    explanationTxt.text = ResultWrong;
                    buttonTexts[1].text = "Try again";
                    currentscreen = currentScreen.Explain;
                }
                break;
            default: break;
        }
    }
    public void topButton()
    {
        audioSource.Play();
        // Only displayed in the initial screen
        currentscreen = currentScreen.Name;
        inputField.SetActive(true);
        buttons[0].SetActive(false);
        changeTexts();
    }

    public void bottomButton()
    {
        audioSource.Play();
        switch (currentscreen)
        {
            case currentScreen.Title:
                SceneManager.LoadScene("Title"); break;
            case currentScreen.Name:
                if (inputText.text == "")
                {
                    explanationTxt.text = "Name cannot\nbe empty.";
                }
                else
                {
                    MemorizationController.playerName = inputText.text;
                    currentscreen = currentScreen.Explain;
                    inputField.SetActive(false);
                    changeTexts();
                }
                break;
            case currentScreen.Explain:
                currentscreen = currentScreen.Practice;
                buttons[1].SetActive(false);
                tutorialObjects.SetActive(false);
                practiceGameObject_Display.SetActive(true);
                StartCoroutine(Practice());
                break;
            case currentScreen.Result:
                currentscreen = currentScreen.Countdown;
                buttons[1].SetActive(false);
                StartCoroutine("CountDown");
                break;
            default: break;
        }
    }
    private void choiceButtonsSetActive(bool a)
    {
        choiceButtons.SetActive(a); ;
        practiceGameObject_SelectAns.SetActive(a);
    }
    IEnumerator Practice()
    {
        for (int i = 0; i < 4; ++i)
        {
            displaySprite.sprite =
                Resources.Load<Sprite>(string.Format(MemorizationController.SpritePathFormat, 1) + imageFiles[i]);
            IndexText.text = (i + 1) + " / 4";
            yield return new WaitForSeconds(MemorizationController.displayInterval);
        }
        practiceGameObject_Display.SetActive(false);
        choiceButtonsSetActive(true);
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
        MemorizationController.instance.init();
        this.gameObject.SetActive(false);
    }
    public void answerButton(int location) // used for answer button
    {
        audioSource.Play();
        if (location == 3) practiceWasCorrect = true;
        else practiceWasCorrect = false;
        currentscreen = currentScreen.Result;
        practiceGameObject_SelectAns.SetActive(false);
        tutorialObjects.SetActive(true);
        buttons[1].SetActive(true);
        choiceButtons.SetActive(false);
        changeTexts();
    }
}
