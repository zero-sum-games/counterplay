using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    int currentScene;
    public GameObject pauseObj;

    GameObject[] destinations;
    bool lerpFX;
    

    void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    private void Update()
    {
        if (lerpFX)
        {

        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("CombatTesting");
    }

    public void ReturnToStart()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void CreditsScreen()
    {
        SceneManager.LoadScene("Credits");
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseObj.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseObj.SetActive(false);
    }

    public void RestartMatch()
    {
        SceneManager.LoadScene(currentScene);
        
    }

    public void CloseGame()
    {
        Application.Quit();
    }

}
