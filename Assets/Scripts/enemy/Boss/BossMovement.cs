using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;

public class BossMovement : MonoBehaviour
{   
    [SerializeField] private float activeDist = 7f;
    [SerializeField] private float bossJumpHeight = 10f;
    [SerializeField] private float bossSpeed = 5f;
    private float bossHealth = 5f;
    private float maxHp = 5f;
    public enum BossState{
        Inactive,
        Fighting,
        Jumping,
        Attacking,
        Stunned,
        Dead
    }
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform platformCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Image healthBar;
    private RaycastHit2D groundHit;
    private RaycastHit2D wallHit;
    private RaycastHit2D platformHit;
    private bool isRight = false;
    public BossState curBossState = BossState.Inactive;
    [SerializeField] private Dialogue startFightDialogue;
    [SerializeField] private Dialogue endFightDialogue;
    [SerializeField] private ScreenShakeProfile profile;
    
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private GameObject[] doors;
    private bool iFrames = false;
    private PlayerMovement pm;
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCol;
    private AudioSource audioSource;
    [SerializeField] private AudioClip enemyHurt;
    [SerializeField] private AudioClip attackWhoosh;
    
    // Start is called before the first frame update
    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider2D>();
        healthBar.fillAmount = bossHealth/maxHp;
        impulseSource = GetComponent<CinemachineImpulseSource>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        groundHit = Physics2D.Raycast(groundCheck.position, -transform.up, 1f, groundLayer);
        platformHit = Physics2D.Raycast(platformCheck.position, transform.up, 2f, groundLayer);
        if(isRight){
            wallHit = Physics2D.Raycast(transform.position, Vector2.right, .5f, groundLayer);
        }else{
            wallHit = Physics2D.Raycast(transform.position, Vector2.left, .5f, groundLayer);
        }
    }

    private void FixedUpdate() {
        if(Vector2.Distance(transform.position, pm.transform.position) < activeDist && curBossState == BossState.Inactive){
            StartCoroutine(StartFight());
        }
        if(curBossState == BossState.Fighting){
            Move();
        }
        
    }

    private IEnumerator StartFight(){
        startFightDialogue.StartDialogue();
        for(int i = 0; i < 2; i++){
            doors[i].GetComponent<Animator>().SetTrigger("close");
        }
        healthBar.enabled = true;
        curBossState = BossState.Stunned;
        while(startFightDialogue.panel.activeInHierarchy){
            yield return null;
        }
        curBossState = BossState.Fighting;
    }
    private void Move(){
        if(curBossState != BossState.Inactive && curBossState != BossState.Stunned && curBossState != BossState.Dead && curBossState != BossState.Attacking){
            if(Vector2.Distance(transform.position, pm.transform.position) < 2f && curBossState != BossState.Attacking){
                StartCoroutine(Attack());
                animator.SetBool("isWalking", false);
            }
            else{
                animator.SetBool("isIdle", false);
                animator.SetBool("isWalking",true);
                if(wallHit != true && curBossState != BossState.Jumping){
                    if(isRight){
                        rb.velocity = new Vector2(bossSpeed, rb.velocity.y);
                    }else{
                        rb.velocity = new Vector2(-bossSpeed, rb.velocity.y);
                    }
                } else{
                    Flip();
                }
                if(platformHit.collider != false && pm.transform.position.y - transform.position.y > 2f && curBossState != BossState.Jumping && Mathf.Abs(pm.transform.position.y - transform.position.y) > 1){
                    StartCoroutine(Jump());
                }
            }
        }

    }
    private void Flip(){

        isRight = !isRight;
        Vector2 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    private IEnumerator Jump(){
        curBossState = BossState.Jumping;
        rb.AddForce(new Vector2(0, bossJumpHeight), ForceMode2D.Impulse);
        yield return new WaitForSeconds(.5f);
        curBossState = BossState.Fighting;
    }
    private IEnumerator Attack(){
        curBossState = BossState.Attacking;
        rb.velocity = Vector2.zero;
        //wait to attack
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.yellow;
        yield return new WaitForSeconds(.1f);
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(.1f);
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.yellow;
        yield return new WaitForSeconds(.1f);
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("attack");
        //attack ths many times
        for(int i = 0; i < 3; i++){
            if(isRight){
                rb.AddForce(Vector2.right*5,ForceMode2D.Impulse);
            }else{
                rb.AddForce(Vector2.left*5,ForceMode2D.Impulse);
            }
            audioSource.clip = attackWhoosh;
            audioSource.Play();
            Debug.Log("attack");
            yield return new WaitForSeconds(.1f);
            rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(.4f);
        }
        curBossState = BossState.Stunned;
        animator.SetBool("isIdle", true);
        pm.iFrames = true;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.magenta;
        yield return new WaitForSeconds(3f);
        pm.iFrames = false;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        curBossState = BossState.Fighting;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("playerAttack")){
            StartCoroutine(TakeDamage());
        }
    }
    IEnumerator TakeDamage(){
        CameraShakeManager.instace.ScreenShakeFromProfile(profile, impulseSource);
        bossHealth--;
        audioSource.clip = enemyHurt;
        audioSource.Play();
        if(!iFrames){
            if(bossHealth < 1 && curBossState != BossState.Dead){
                StartCoroutine(EndGame());
            }else{
                if(curBossState == BossState.Stunned){
                    transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
                }
                healthBar.fillAmount = bossHealth/maxHp;
                iFrames = true;
                pm.iFrames = false;
                curBossState = BossState.Fighting;
                yield return new WaitForSeconds(.5f);
                iFrames = false;
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
            } 
        }

    }
    IEnumerator EndGame(){
        animator.SetTrigger("dead");
        curBossState = BossState.Dead;
        boxCol.enabled = false;
        healthBar.enabled = false;
        endFightDialogue.StartDialogue();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        this.enabled = false;
        yield return new WaitForSeconds(1.5f);
        for(int i = 0; i < 2; i++){
            doors[i].GetComponent<Animator>().SetTrigger("open");
        }
    }
}
