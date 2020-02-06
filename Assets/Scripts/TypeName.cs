using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class TypeName : MonoBehaviour
{
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private GameObject loadingScreen;

    private void Start()
    {
        loadingScreen.SetActive(false);
    }
    public void ButtonPressed()
    {
        Setting.playerName = nameText.text;
        loadingScreen.SetActive(true);
        Setting.goToNextScene();
    }
}
