using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Operation {add = 0, subtract, muptiply, divide};
public class Calculation
{
    private int operand1;
    private int operand2;
    public int actualAnswer;
    private Operation operation;
    public int[] choices; // Store 3 wrong answers
    public int answerByPlayer;
    public bool haveCarry;
    public bool haveBorrow;
    private const string COMMA = ",";

    private void shuffleAndPopulateChoices(int[] tempChoices)
    {
        for (int n = 5; n >= 1; --n) { 
            int k = Random.Range(0, n+1);
            int value = tempChoices[k];
            tempChoices[k] = tempChoices[n];
            tempChoices[n] = value;
        }
        for (int i = 0; i < 3; ++i)
        {
            choices[i] = tempChoices[i];
        }
    }
    private void makeAddCalculation()
    {
        actualAnswer = operand1 + operand2;
        int[] tempChoices = new int[6];
        tempChoices[0] = actualAnswer + 1;
        tempChoices[1] = actualAnswer - 1;
        tempChoices[2] = actualAnswer + 2;
        tempChoices[3] = actualAnswer - 2;
        tempChoices[4] = actualAnswer + 10;
        tempChoices[5] = actualAnswer - 10;
        shuffleAndPopulateChoices(tempChoices);
    }
    private void makeSubtractCalculation()
    {
        actualAnswer = operand1 - operand2;
        int[] tempChoices = new int[6];
        tempChoices[0] = actualAnswer + 1;
        tempChoices[1] = actualAnswer - 1;
        tempChoices[2] = actualAnswer + 2;
        tempChoices[3] = actualAnswer - 2;
        tempChoices[4] = actualAnswer + 10;
        tempChoices[5] = actualAnswer - 10;
        shuffleAndPopulateChoices(tempChoices);
    }
    private void makeMultiplyCalculation()
    {
        actualAnswer = operand1 * operand2;
        int[] tempChoices = new int[6];
        tempChoices[0] = (operand1 - 1) * operand2;
        tempChoices[1] = operand1 * (operand2 + 1);
        tempChoices[2] = operand1 + operand2;
        tempChoices[3] = Mathf.Abs(operand1 - operand2);
        tempChoices[4] = operand1 * (operand2 - 1);
        tempChoices[5] = (operand1 + 1) * operand2;
        shuffleAndPopulateChoices(tempChoices);
    }
    private void makeDivideCalculation()
    {
        actualAnswer = operand1 / operand2;
        int[] tempChoices = new int[6];
        tempChoices[0] = actualAnswer - 1;
        tempChoices[1] = actualAnswer + 1;
        tempChoices[2] = operand1 + operand2;
        tempChoices[3] = Mathf.Abs(operand1 - operand2);
        tempChoices[4] = actualAnswer - 2;
        tempChoices[5] = actualAnswer + 2;
        shuffleAndPopulateChoices(tempChoices);
    }
    public string calculationString()
    {
        string str = "";
        switch (operation)
        {
            case Operation.add: str = "+"; break;
            case Operation.subtract: str = "-"; break;
            case Operation.muptiply: str = "X"; break;
            case Operation.divide: str = "/"; break;
            default: break;
        }
        return operand1.ToString() + " " + str + " " + operand2.ToString();
    }
    public string resultString()
    {
        return operand1.ToString() + COMMA + 
            operand2.ToString() + COMMA +
            ((int)operation).ToString() + COMMA +
            actualAnswer.ToString() + COMMA + 
            answerByPlayer.ToString();
        //Operand 1,Operand 2,Operation,Correct Answer, Chosen Answer
    }
    public Calculation(int a, int b, Operation oper)
    {
        operand1 = a;
        operand2 = b;
        operation = oper;
        choices = new int[3];

        switch (oper)
        {
            case Operation.add: makeAddCalculation(); break;
            case Operation.subtract: makeSubtractCalculation(); break;
            case Operation.muptiply: makeMultiplyCalculation(); break;
            case Operation.divide: makeDivideCalculation(); break;
            default: break;
        }
    }

}
