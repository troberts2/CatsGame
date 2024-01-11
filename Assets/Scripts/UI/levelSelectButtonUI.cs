using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class levelSelectButtonUI : MonoBehaviour
{
    private GameObject uiPopUp;
    private GameObject levelSelectMainMenu;
    // Start is called before the first frame update
    void Start()
    {
        uiPopUp = transform.GetChild(0).gameObject;
        levelSelectMainMenu = transform.parent.parent.GetChild(transform.parent.parent.childCount -1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void levelButtonClicked(){
        uiPopUp.SetActive(true);
        levelSelectMainMenu.SetActive(false);
    }
    public void BackToLevelSelect(){
        uiPopUp.SetActive(false);
        levelSelectMainMenu.SetActive(true);

    }
}
