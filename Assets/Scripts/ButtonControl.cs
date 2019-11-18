using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[System.Serializable]
public class ButtonControl : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField]
    private GameObject loading;
    private void Start()
    {
        loading.SetActive(false);
        audioSource = this.GetComponent<AudioSource>();
    }
    public void Memorization()
    {
        loading.SetActive(true);
        audioSource.Play();
        SceneManager.LoadScene("Memorization");
    }
    public void Calculation()
    {
        loading.SetActive(true);
        audioSource.Play();
        SceneManager.LoadScene("Calculation");
    }
    public void Attention()
    {
        loading.SetActive(true);
        audioSource.Play();
        SceneManager.LoadScene("Attention");
    }

    public void Setting()
    {
        loading.SetActive(true);
        audioSource.Play();
        SceneManager.LoadScene("Setting");
    }
}
