using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject pauseMenu, continueButton, mainMenuButton;
    void Update()
    {
        if(gameObject.name != "Button1")
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(pauseMenu.activeSelf == false)
                {
                    pauseMenu.SetActive(true);
                    Time.timeScale = 0f;
                }
                else
                {
                    pauseMenu.SetActive(false);
                    Time.timeScale = 1f;
                }
            }
        }
    }

    public void OnContinueButtonClicked()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnMainMenuButtonClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
