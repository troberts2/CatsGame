using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButton : MonoBehaviour
{
    PlayerMovement pm;
    [SerializeField] private GameObject optionsButton;


    private void Start() {
        pm = FindObjectOfType<PlayerMovement>();
    }
    public void ResumeGame(){
        pm.pauseMenu.SetActive(false);
        Time.timeScale = 1;
        pm.isPaused = false;
    }
    public void Options(){
        optionsButton.SetActive(true);
        pm.pauseMenu.SetActive(false);
    }
    public void MainMenu(){
        pm.SavePlayer();
        SceneManager.LoadScene("TitleScreen");
        Time.timeScale = 1;
    }

    public void LoadLevelByName(string name){
        SceneManager.LoadScene(name);
    }

    public void ReloadScene(){
        pm.SavePlayer();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
