using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
    public ParticleSystem blood;
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private ScreenShakeProfile profile;
    [SerializeField] private GameObject scoreText;
    private AudioSource audioSource;
    [SerializeField] private AudioClip enemyHurt;
    private void Start(){
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCol = GetComponentInChildren<BoxCollider2D>();
        impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
        audioSource = GetComponent<AudioSource>();
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
            if(FindObjectOfType<PlayerMovement>().transform.position.y + 5 < transform.position.y){
                transform.Translate (Vector2.down * 2f * Time.deltaTime);
            }
        }
        if(rb.IsSleeping()){
            rb.WakeUp();
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("playerAttack")){
            StartCoroutine(Die());
        }
    }
    IEnumerator Die(){
        FindObjectOfType<PlayerMovement>().score += 100;
        audioSource.clip = enemyHurt;
        audioSource.Play();
        if(!dirRight){
            transform.localScale = new Vector3(-transform.localScale.x, 1f, 1f);
        }
        scoreText.SetActive(true);
        scoreText.transform.SetParent(null);
        Destroy(scoreText, .5f);
        animator.SetTrigger("dead");
        blood.Play();
        CameraShakeManager.instace.ScreenShakeFromProfile(profile, impulseSource);
        boxCol.enabled = false;
        canMove = false;
        rb.gravityScale = 1f;
        rb.mass = 1f;
        rb.velocity = Vector2.zero;
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
