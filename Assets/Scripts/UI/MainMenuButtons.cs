using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void playGame(){
        SceneManager.LoadScene("LevelSelect");
    }
    public void options(){
        SceneManager.LoadScene("Options");
    }
    public void Exit(){
        Application.Quit();
    }
}
