using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class KeithEnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCol;
    public float moveSpeed;
    private Animator animator;
    private bool canMove = false;
    public ParticleSystem blood;
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate(){
        if(Vector2.Distance(transform.position, FindObjectOfType<PlayerMovement>().transform.position) < 15 && Vector2.Distance(transform.position, FindObjectOfType<PlayerMovement>().transform.position) > 9){
            canMove = true;
        }
        if(canMove){
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }

    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("playerAttack")){
            StartCoroutine(Die());
        }
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player")){
            StartCoroutine(BounceBack(other.gameObject));
        }
    }

    IEnumerator Die(){
        animator.SetTrigger("dead");
        blood.Play();
        boxCol.enabled = false;
        rb.isKinematic = false;
        canMove = false;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.AddForce(new Vector2(0, 2f), ForceMode2D.Impulse);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    IEnumerator BounceBack(GameObject player){
        canMove = false;
        rb.isKinematic = false;
        Vector2 forceDir = gameObject.transform.position - player.transform.position;
        forceDir *= 4;
        rb.AddForce(forceDir, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1f);
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        canMove = true;
    }
}
