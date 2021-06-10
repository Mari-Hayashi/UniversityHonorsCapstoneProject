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
        switch (SceneManager.GetActiveScene().name)
        {
            case "DataPath":
                text.text = "The data file is in the folder:\n\n" + Application.persistentDataPath;
                break;
            case "SessionDone":
                text.text = "Good job! Your training for today is done.\n\nThe data file is outputted to: " + Application.persistentDataPath;
                break;
            default:
                break;
        }
        }

    public void DoneButtonPressed()
    {
        SceneManager.LoadScene("Title");
    }
}
