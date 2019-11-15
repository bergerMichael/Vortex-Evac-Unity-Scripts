using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("IntroScene");
    }

    public void LoadWinScene()
    {
        SceneManager.LoadScene("WinScene");
    }
    public void LoadLoseScene()
    {
        SceneManager.LoadScene("LoseScene");
    }
    public void LoadTutorial()
    {
        SceneManager.LoadScene("TutorialScene");
    }
}
