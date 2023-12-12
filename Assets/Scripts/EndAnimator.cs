using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndAnimator : MonoBehaviour
{
    private PlayerMovement pm;
    private beanManager bm;
    public Image[] beans;
    [SerializeField] private GameObject EndScreenUI;
    private IDataService DataService = new JsonDataService();
    private LevelInfo levelInfo = new LevelInfo();
    private Animator animator;
    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        bm = FindObjectOfType<beanManager>();
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.CompareTag("Player")){
            StartCoroutine(CollideWithP());
            animator.SetTrigger("end");
        }
    }

    IEnumerator CollideWithP(){
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        pm.GetComponent<PlayerMovement>().enabled = false;
        pm.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(2.5f);
        EndScreenUI.SetActive(true);
        for(int i = 0; i < bm.beanVal.Length; i++){
            if(bm.beanVal[i]){
                beans[i].sprite = bm.beansClear;
            }
        }
        levelInfo.beansColl = bm.beanVal;
        pm.infoUI.SetActive(false);
        SerializeJson();
    }
        public void SerializeJson(){
            DataService.SaveData("/" + SceneManager.GetActiveScene().name + ".json", levelInfo, false);
    }
}
