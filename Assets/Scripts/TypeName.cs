using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class TypeName : MonoBehaviour
{
    [SerializeField]
    private Text nameText;

    public void ButtonPressed()
    {
        Setting.playerName = nameText.text;
        Setting.goToNextScene();
    }
}
