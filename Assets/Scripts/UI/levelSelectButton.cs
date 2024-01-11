using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class levelSelectButton : MonoBehaviour
{
    private IDataService DataService = new JsonDataService();
    [SerializeField] private string prevLevel;
    [SerializeField] private string thisLevel;
    private bool unlocked = false;
    private Button button;
    [SerializeField] private Image[] beans;
    [SerializeField] private TextMeshProUGUI clearedText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Sprite beanClear;
    private void Start() {
        button = GetComponent<Button>();
        LoadJson();
    }
    public void LoadJson(){
        string path = Application.persistentDataPath + "/" + prevLevel + ".json";
        string path2 = Application.persistentDataPath + "/" + thisLevel + ".json";
        if(File.Exists(path2)){
            LevelInfo dataThis = DataService.LoadData<LevelInfo>("/" + thisLevel + ".json", false);
            button.interactable = true;
            for(int i = 0; i < beans.Length; i++){
                if(dataThis.beansColl[i]){
                    beans[i].sprite = beanClear;
                }
            }
            if(dataThis.levelBeat){
                clearedText.color = Color.green;
                clearedText.text = "Cleared";
            }
            if(dataThis.highScore > 0){
                scoreText.text = "High Score: " + dataThis.highScore;
            }else{
                scoreText.text = "High Score: 0";
            }
        }else{
            if(File.Exists(path)){
                LevelInfo dataPrev = DataService.LoadData<LevelInfo>("/" + prevLevel + ".json", false);
                if(dataPrev.levelBeat){
                    StartCoroutine(UnlockButton());
                }
            }
        }
    }

    IEnumerator UnlockButton(){
        yield return new WaitForSeconds(2f);
        button.interactable = true;
    }
}
