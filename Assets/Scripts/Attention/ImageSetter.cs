using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ImageSetter : MonoBehaviour
{
    [SerializeField]
    private GameObject[] buttons;
    private Image[] buttonImages;
    private RectTransform[] buttonTransforms;

    [SerializeField]
    private GameObject[] circles;

    [SerializeField]
    private Sprite apple1;
    [SerializeField]
    private Sprite apple2;
    [SerializeField]
    private Sprite orange1;
    [SerializeField]
    private Sprite orange2;

    private Vector3 rotationVector;
    private Vector3 nonRotationVector;
    private Color transparentColor;
    
    private void Awake()
    {
        rotationVector = new Vector3(0f, 0f, 180f);
        nonRotationVector = new Vector3(0f, 0f, 0f);
        transparentColor = new Color(0f, 0f, 0f, 0.5f);

        buttonImages = new Image[AttentionController.numImages];
        buttonTransforms = new RectTransform[AttentionController.numImages];
        for (int i = 0; i < AttentionController.numImages; ++i)
        {
            buttonImages[i] = buttons[i].GetComponent<Image>();
            buttonTransforms[i] = buttons[i].GetComponent<RectTransform>();
        }
    }

    public void SetImages(string[] str)
    {
        if (str.Length != AttentionController.numImages)
        {
            Debug.Log("SetImages() called on wrong string[]size.");
            return;
        }
        for (int i = 0; i < AttentionController.numImages; ++i)
        {
            setSpriteFromString(str[i], i);
        }
        displayCircle(-1);
    }
    private void setSpriteFromString(string str, int i)
    {
        if (i < 0 || i >= AttentionController.numImages) return;

        string fruitStr = str[0].ToString() + str[1].ToString();
        Sprite sprite;
        switch (fruitStr)
        {
            case "00": sprite = apple1; break;
            case "01": sprite = apple2; break;
            case "10": sprite = orange1; break;
            case "11": sprite = orange2; break;
            default: return;
        }
        buttonImages[i].sprite = sprite;
        if (str[2] == '1') buttonTransforms[i].rotation = Quaternion.Euler(rotationVector);
        else buttonTransforms[i].rotation = Quaternion.Euler(nonRotationVector);
    }

    public void buttonPressed(int i)
    {
        if (!AttentionController.gameGoingOn)
        {
            SceneManager.LoadScene("Title");
        }
        displayCircle(i);
        AttentionController.instance.buttonPressed(i);
    }

    public void displayCircle(int loc)
    {
        if (loc < AttentionController.numImages && loc > -1)
        {
            circles[loc].SetActive(true);
        } else
        {
            for (int i = 0; i < circles.Length; ++i)
            {
                circles[i].SetActive(false);
            }
        }
    }
}
