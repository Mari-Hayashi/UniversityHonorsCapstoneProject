using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MemorizationQuestion
{
    public string[] sprites;
    public string[] choices;
    public int numSprites;
    public int answerIndexInSprites;
    public int difficulty;
    public string answerSelected;

    public Sprite getDisplaySprite(int num)
    {
        return Resources.Load<Sprite>(MemorizationController.instance.SpritePath + sprites[num]);
    }
    public Sprite getChoiceSprite(int num)
    {
        return Resources.Load<Sprite>(MemorizationController.instance.SpritePath + choices[num]);
    }
    public MemorizationQuestion(string[] imageFileNames, string[] choiceFileNames, 
        string answerFileName, int size, int diff)
    {
        numSprites = size;
        difficulty = diff;
        sprites = new string[numSprites];
        
        sprites = imageFileNames;
        choices = choiceFileNames;
        for (int i = 0; i < numSprites; ++i)
        {
            if (sprites[i] == answerFileName)
            {
                answerIndexInSprites = i;
                break;
            }
        }
    }
    public string indexString()
    {
        switch (answerIndexInSprites)
        {
            case 0: return "1st";
            case 1: return "2nd";
            case 2: return "3rd";
            default: return answerIndexInSprites.ToString() + "th";
        }
    }
    private void shuffleString(string[] array, int size)
    {
        for (int n = size - 1; n >= 1; --n)
        {
            int k = UnityEngine.Random.Range(0, n + 1);
            string value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
    }
    public void gotAnswer(int ans)
    {
        answerSelected = choices[ans];
    }
    public bool isCorrect()
    {
        return answerSelected == sprites[answerIndexInSprites];
    }
    public string choicesString()
    {
        string str = "";
        for (int i = 0; i < MemorizationController.numChoices; ++i)
        {
            str += choices[i] + MemorizationController.COMMA;
        }
        return str;
    }
    public string spritesString()
    {
        string str = "";
        for (int i = 0; i < numSprites; ++i)
        {
            str += sprites[i] + MemorizationController.COMMA;
        }
        return str;
    }
    // Resources.Load<Sprite>(MemorizationController.SpritePath + imageFileNames[i]);
}
