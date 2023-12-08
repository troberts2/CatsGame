using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class KeithEnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCol;
    public float moveSpeed;
    private Animator animator;
    private bool canMove = false;
    public ParticleSystem blood;
    [SerializeField] private ScreenShakeProfile profile;
    [SerializeField] private LayerMask groundLayer;
    private CinemachineImpulseSource impulseSource;
    private Transform groundCheck;
    private RaycastHit2D hit;
    private bool isFacingRight = false;
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        groundCheck = transform.GetChild(0);
    }
    private void Update() {
        hit = Physics2D.Raycast(groundCheck.position, Vector2.down, .2f, groundLayer);
    }

    private void FixedUpdate(){
        if(Vector2.Distance(transform.position, FindObjectOfType<PlayerMovement>().transform.position) < 15 && Vector2.Distance(transform.position, FindObjectOfType<PlayerMovement>().transform.position) > 9){
            canMove = true;
        }
        if(canMove){
            if (hit.collider !=  null) {
                if (isFacingRight) {
                    transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
                }else {
                    transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
                }
            }else{
                Debug.Log("not");
                isFacingRight = !isFacingRight;
                transform.localScale = new Vector3(-transform.localScale.x, 1f, 1f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("playerAttack")){
            StartCoroutine(Die(other.gameObject.transform.parent.parent.gameObject));
        }
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player")){
            StartCoroutine(BounceBack(other.gameObject));
        }
        if(other.gameObject.CompareTag("enemy")){
                isFacingRight = !isFacingRight;
                transform.localScale = new Vector3(-transform.localScale.x, 1f, 1f);
        }
    }

    IEnumerator Die(GameObject player){
        animator.SetTrigger("dead");
        blood.Play();
        CameraShakeManager.instace.ScreenShakeFromProfile(profile, impulseSource);
        boxCol.enabled = false;
        rb.isKinematic = false;
        canMove = false;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.AddForce(new Vector2(0, 2f), ForceMode2D.Impulse);
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, 0);
        Vector2 forceDir = player.transform.position - gameObject.transform.position;
        forceDir = new Vector2(0, forceDir.y);
        forceDir *= 6;
        player.GetComponent<Rigidbody2D>().AddForce(forceDir, ForceMode2D.Impulse);
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
