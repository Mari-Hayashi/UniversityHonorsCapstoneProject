using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttentionQuestion : MonoBehaviour
{
    public string[] ImageString;
    public float[] ResponceTime;
    public AttentionQuestion(string[] numbers)
    {
        ImageString = new string[AttentionController.numImages];
        ImageString = numbers;
        shuffleImageSequence();
        ResponceTime = new float[AttentionController.numImages];
        for (int i = 0; i < AttentionController.numImages; ++i)
        {
            ResponceTime[i] = 0f;
        }
    }
    private void shuffleImageSequence()
    {
        for (int n = AttentionController.numImages - 1; n >= 1; --n)
        {
            int k = Random.Range(0, n + 1);
            string value = ImageString[k];
            ImageString[k] = ImageString[n];
            ImageString[n] = value;
        }
    }
    public string responceTimeCSV()
    {
        string returnString = "";
        string comma = ",";
        for (int i = 0; i < AttentionController.numImages; ++i)
        {
            returnString += ImageString[i] + comma + ResponceTime[i] + comma;
        }
        return returnString;
    }
}
