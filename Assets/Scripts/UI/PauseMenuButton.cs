using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButton : MonoBehaviour
{
    PlayerMovement pm;
    [SerializeField] private GameObject optionsButton;
    [SerializeField] private AudioManager audioManager;


    private void Start() {
        pm = FindObjectOfType<PlayerMovement>();
        audioManager = FindObjectOfType<AudioManager>();
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
    public void OptionsMenu(){
        optionsButton.SetActive(true);
    }
    public void MainMenu(){
        pm.SavePlayer();
        audioManager.SaveVolume();
        SceneManager.LoadScene("TitleScreen");
        Time.timeScale = 1;
    }

    public void LoadLevelByName(string name){
        audioManager.SaveVolume();
        SceneManager.LoadScene(name);
    }

    public void ReloadScene(){
        pm.SavePlayer();
        audioManager.SaveVolume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
