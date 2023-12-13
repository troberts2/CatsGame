using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAnimator : MonoBehaviour
{
    private PlayerMovement pm;
    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        StartCoroutine(ShowPlayer());
    }
    IEnumerator ShowPlayer(){
        yield return new WaitForSeconds(3.5f);
        pm.GetComponent<SpriteRenderer>().enabled = true;
        pm.animator.SetTrigger("dash");
        yield return new WaitForSeconds(.5f);
        pm.GetComponent<PlayerMovement>().enabled = true;
    }


}
