using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class beanManager : MonoBehaviour
{
    //UI
    public Image[] beans;
    [SerializeField] private Sprite beansClear;
    [SerializeField] private Sprite darkBeans;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateBeansUI(int index){
        beans[index].sprite = beansClear;
    }
}
