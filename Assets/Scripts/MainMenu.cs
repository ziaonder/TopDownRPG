using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject save1, save2, save3, saveInfoHolder;
    public TextMeshProUGUI textMesh;

    private void Awake()
    {
        Time.timeScale = 1f;
    }
    public void OnPressPlayButton()
    {
        save1.SetActive(true);
        save2.SetActive(true);
        save3.SetActive(true);
    }

    public void OnDeleteButtonPressed()
    {
        if (gameObject.name.Contains("Save1"))
            SaveSystem.DeleteSave("1");
        else if(gameObject.name.Contains("Save2"))
            SaveSystem.DeleteSave("2");
        else
            SaveSystem.DeleteSave("3");

        textMesh.text = "Save Deleted!";
        StartCoroutine(PopSaveInfoHolder());
    }

    private IEnumerator PopSaveInfoHolder()
    {
        saveInfoHolder.SetActive(true);
        yield return new WaitForSeconds(2f);
        saveInfoHolder.SetActive(false);
    }
    public void OnPressQuitButton()
    {
        Application.Quit();
    }

    public void OnPressSave1Button()
    {
        PlayerPrefs.SetInt("SaveSlot", 1);
        if (PlayerPrefs.GetString(1.ToString()) == "saved")
            textMesh.text = "Save Found!";
        else
            textMesh.text = "New Game!";
        saveInfoHolder.SetActive(true);

        StartCoroutine(LoadScene());
    }

    public void OnPressSave2Button()
    {
        PlayerPrefs.SetInt("SaveSlot", 2);
        if (PlayerPrefs.GetString(2.ToString()) == "saved")
            textMesh.text = "Save Found!";
        else
            textMesh.text = "New Game!";
        saveInfoHolder.SetActive(true);

        StartCoroutine(LoadScene());
    }

    public void OnPressSave3Button()
    {
        PlayerPrefs.SetInt("SaveSlot", 3);
        if (PlayerPrefs.GetString(3.ToString()) == "saved")
            textMesh.text = "Save Found!";
        else
            textMesh.text = "New Game!";
        saveInfoHolder.SetActive(true);

        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2);
        saveInfoHolder.SetActive(false);
        SceneManager.LoadScene("Game");
    }
}
