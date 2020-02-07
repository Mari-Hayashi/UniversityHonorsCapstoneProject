using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DoneController : MonoBehaviour
{
    [SerializeField]
    private Text text;
    
    void Start()
    {
        string fileDirectry = Application.persistentDataPath + "/";
        text.text = "Good job! your training for today is done.\n\nPlease hand the device to RA.\n\nYour record file is generated in: " + fileDirectry;
    }
    public void DoneButtonPressed()
    {
        SceneManager.LoadScene("Title");
    }
}
