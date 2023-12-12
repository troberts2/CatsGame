using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class beanManager : MonoBehaviour
{
    //UI
    public Image[] beans;
    internal bool[] beanVal = new bool[3];
    [SerializeField] internal Sprite beansClear;
    [SerializeField] private Sprite darkBeans;
    private IDataService DataService = new JsonDataService();
    // Start is called before the first frame update
    void Start()
    {
        LoadJson();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateBeansUI(int index){
        beans[index].sprite = beansClear;
        beanVal[index] = true;
    }
    public void LoadJson(){
        string path = Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + ".json";
        if(File.Exists(path)){
            LevelInfo data = DataService.LoadData<LevelInfo>("/" + SceneManager.GetActiveScene().name + ".json", false);
            beanVal = data.beansColl;
            for(int i = 0; i < beanVal.Length; i++){
                if(beanVal[i]){
                    beans[i].sprite = beansClear;
                }
            }  
        }
    }
}
