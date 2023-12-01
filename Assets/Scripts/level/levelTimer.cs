using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class levelTimer : MonoBehaviour
{
    public float timeRemaining;
    public bool timerIsRunning;
    public TextMeshProUGUI timerText;
    // Start is called before the first frame update
    void Start()
    {
        timerIsRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(timerIsRunning){
            if(timeRemaining > 0){
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }else{
                Debug.Log("Time has run out");
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }
    private void DisplayTime(float timeToDisplay){
        int seconds = (int)timeToDisplay;
        timerText.text = "" + seconds;
    }
}
