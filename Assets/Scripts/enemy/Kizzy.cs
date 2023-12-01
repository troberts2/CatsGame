using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Kizzy : MonoBehaviour
{
    public float attackRate;
    private bool canMove = false;
    private bool dirRight = true;
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCol;
    public GameObject poop;
    private void Start(){
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCol = GetComponentInChildren<BoxCollider2D>();
    }
    private void Awake(){
        StartCoroutine(Poo());
    }
    private void Update(){
        if(Vector2.Distance(transform.position, FindObjectOfType<PlayerMovement>().transform.position) < 15 && Vector2.Distance(transform.position, FindObjectOfType<PlayerMovement>().transform.position) > 9){
            canMove = true;
        }
        if(canMove){
            
            Vector2 pointA = new Vector2(FindObjectOfType<PlayerMovement>().transform.position.x + 6f, transform.position.y);
            Vector2 pointB = new Vector2(FindObjectOfType<PlayerMovement>().transform.position.x - 6f, transform.position.y);
            if (dirRight){
                transform.Translate (Vector2.right * 2f * Time.deltaTime);
                transform.GetComponentInChildren<SpriteRenderer>().flipX = true;
            }
            else{
                transform.Translate (-Vector2.right * 2f * Time.deltaTime);
                transform.GetComponentInChildren<SpriteRenderer>().flipX = false;
            }
            
            if(transform.position.x >= pointA.x) {
                dirRight = false;
            }
            
            if(transform.position.x <= pointB.x) {
                dirRight = true;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("playerAttack")){
            StartCoroutine(Die());
        }
    }
    IEnumerator Die(){
        animator.SetTrigger("dead");
        boxCol.enabled = false;
        canMove = false;
        rb.gravityScale = 1f;
        rb.mass = 1f;
        rb.AddForce(new Vector2(0, 2f), ForceMode2D.Impulse);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    IEnumerator Poo(){
        GameObject poo = Instantiate(poop, transform.position, Quaternion.identity);
        Destroy(poo, 5f);
        yield return new WaitForSeconds(attackRate);
        StartCoroutine(Poo());
    }
}
