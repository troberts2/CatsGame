using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.IO;

public class PlayerMovement : MonoBehaviour
{
    //movement and physics;
    private Rigidbody2D rb;
    public float moveSpeed;
    public float jumpForce;
    public float dashForce;
    private bool isGrounded;
    private int jumpCount;
    private int jumpCountMax = 2;
    private Vector2 lastPosJump;
    private int dashCount = 2;
    private int dashCountMax = 2;
    public LayerMask groundLayer;
    //Player status
    [SerializeField]private int health = 5;
    [SerializeField]private int maxHealth = 5;
    internal int lives = 5;
    private bool iFrames = false;

    //Input actions
    public PlayerInputActions playerControls;
    internal Vector2 moveDir = Vector2.zero;
    private InputAction move;
    private InputAction jump;
    private InputAction dash;
    private InputAction pencil;
    private InputAction sprinkle;
    private InputAction squigle;
    private InputAction pause;

    //UI
    public Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private TextMeshProUGUI livesText;
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private ScreenShakeProfile profile;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject gameoverScreen;

    //other
    internal Animator animator;
    public ParticleSystem dust;
    public ParticleSystem blood;
    public ParticleSystem[] pencilAttack;
    public ParticleSystem sprinkleAttack;
    public ParticleSystem[] squigleAttack;
    
    //references
    private beanManager bm;
    private IDataService DataService = new JsonDataService();
    private PlayerInfo playerInfo = new PlayerInfo();
    [HideInInspector]public GameObject pauseMenu;
    [HideInInspector]public GameObject infoUI;
    private GameObject optionsMenu;
    public bool isPaused = false;

    void Awake(){
        bm = FindObjectOfType<beanManager>();
        playerControls = new PlayerInputActions();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        pauseMenu = transform.GetChild(0).GetChild(0).gameObject;
        optionsMenu = transform.GetChild(0).GetChild(1).gameObject;
        infoUI = transform.GetChild(1).gameObject;
        LoadJson();
    }

    private void OnEnable(){
        //sets up the playercontrols
        move = playerControls.Player.Move;
        move.Enable();
        move.performed += MovePerformed;
        move.canceled += MoveCancelled;

        jump = playerControls.Player.Jump;
        jump.Enable();
        jump.performed += Jump;

        dash = playerControls.Player.Dash;
        dash.Enable();
        dash.performed += Dash;

        pencil = playerControls.Player.Pencil;
        pencil.Enable();
        pencil.performed += Pencil;

        sprinkle = playerControls.Player.Sprinkle;
        sprinkle.Enable();
        sprinkle.performed += Sprinkle;

        squigle = playerControls.Player.Squigle;
        squigle.Enable();
        squigle.performed += Squigle;
        
        pause = playerControls.Player.Pause;
        pause.Enable();
        pause.performed += Pause;
    }

    private void OnDisable(){
        //disables player controls
        move.Disable();
        move.performed -= MovePerformed;
        move.canceled -= MoveCancelled;
        jump.Disable();
        dash.Disable();
        pencil.Disable();
        sprinkle.Disable();
        squigle.Disable();
        pause.Disable();
    }

    private void Update() {
        checks();
        UIUpdate();
    }

    /// <summary>
    /// currently just moves the player
    /// </summary>
    private void FixedUpdate() {
        //basic movement with addforce
        rb.AddForce(new Vector2(moveDir.x * moveSpeed, 0), ForceMode2D.Force);
    }
    private void MovePerformed(InputAction.CallbackContext callbackContext){
        if(!isPaused){
            moveDir = callbackContext.ReadValue<Vector2>();
            if(moveDir.x > 0){
                transform.localScale = Vector2.one;
                if(IsGrounded()){
                    dust.Play();
                }
            }else{
                transform.localScale = new Vector2(-1f, 1f);
                if(IsGrounded()){
                    dust.Play();
                }
            }
            animator.SetBool("isRunning", true); 
        }

    }
    private void MoveCancelled(InputAction.CallbackContext callbackContext){
        moveDir = Vector2.zero;
        animator.SetBool("isRunning", false);
    }
    /// <summary>
    /// jump functionallity. Jumps as many times as want just change max jump count to as many jumps
    /// they should be aloud.
    /// </summary>
    /// <param name="context"></param>
    private void Jump(InputAction.CallbackContext context){
        if(!isPaused){
            if(jumpCount > 1){
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpCount--;
                animator.SetTrigger("jump");
                dust.Play();
            }
        }

    }
    /// <summary>
    /// dash functionallity. Adds force as long as player is moving in a direction
    /// </summary>
    /// <param name="context"></param>
    private void Dash(InputAction.CallbackContext context){
        if(!isPaused){
            if(dashCount > 0 && moveDir != Vector2.zero){
                Debug.Log("dash");
                rb.AddForce(moveDir * dashForce, ForceMode2D.Impulse);
                dashCount--;
                animator.SetTrigger("dash");
                StartCoroutine(DashTimer());
                StartCoroutine(DashConstraint());
            }
        }

    }
    private void Pencil(InputAction.CallbackContext callbackContext){
        if(!isPaused){
                animator.SetTrigger("pencilAttack");
                foreach(ParticleSystem ps in pencilAttack){
                ps.Play();
            }
        }
  
    }
    private void Sprinkle(InputAction.CallbackContext callbackContext){
        if(!isPaused){
            animator.SetTrigger("sprinkleAttack");
            sprinkleAttack.Play();
        }

    }
    private void Squigle(InputAction.CallbackContext callbackContext){
        if(!isPaused){
            animator.SetTrigger("squigleAttack");
            foreach(ParticleSystem ps in squigleAttack){
                ps.Play();
            }
        }

    }
    private void Pause(InputAction.CallbackContext callbackContext){
        if(pauseMenu.activeInHierarchy){
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            isPaused = false;
        }else if(optionsMenu.activeInHierarchy){
            pauseMenu.SetActive(true);
            optionsMenu.SetActive(false);
        }
        else{
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            isPaused = true;
        }
    }
    /// <summary>
    /// resets the jump count
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.CompareTag("enemy")){
            if(!iFrames){
                if(health > 1){
                    StartCoroutine(TakeDamage());
                }
                else{
                    StartCoroutine(Die());
                }
            }
        }
        if(other.collider.CompareTag("ground")){
            dust.Play();
        }
        if(other.collider.CompareTag("hazard")){
            transform.position = lastPosJump;
            if(health > 1){
                StartCoroutine(TakeDamage());
            }
            else{
                StartCoroutine(Die());
            }

        }
    }
    private void OnCollisionExit2D(Collision2D other) {
        if(other.collider.CompareTag("ground")){
            lastPosJump = transform.position;
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("greenBean")){
            bm.UpdateBeansUI(other.GetComponent<bean>().id);
            Destroy(other.gameObject, 0.2f);
        }
        if(other.CompareTag("poo")){
            if(!iFrames){
                if(health > 1){
                    StartCoroutine(TakeDamage());
                }
                else{
                    StartCoroutine(Die());
                }
            }
        }
        if(other.CompareTag("dialogue")){
            other.GetComponentInChildren<Dialogue>().StartDialogue();
        }
        if(other.CompareTag("coin")){
            Destroy(other.gameObject);
        }
    }
    /// <summary>
    /// dash timer and count
    /// </summary>
    /// <returns></returns>
    private IEnumerator DashTimer(){
        yield return new WaitForSeconds(2f);
        if(dashCount < dashCountMax){
            dashCount++;
        }
    }
    /// <summary>
    /// sets gravity to 0 while dashing intially to let player have a more floaty dash
    /// </summary>
    /// <returns></returns>
    private IEnumerator DashConstraint(){
        float tempGravScale = rb.gravityScale;
        rb.gravityScale = 0;
        yield return new WaitForSeconds(.3f);
        rb.gravityScale = tempGravScale;
    }
    private IEnumerator TakeDamage(){
        animator.SetTrigger("hurt");
        blood.Play();
        //CameraShakeManager.instace.CameraShake(impulseSource);
        CameraShakeManager.instace.ScreenShakeFromProfile(profile, impulseSource);
        health--;
        playerInfo.health = health;
        iFrames = true;
        yield return new WaitForSeconds(.5f);
        iFrames = false;
    }
    private IEnumerator Die(){
        animator.SetTrigger("dead");
        blood.Play();
        this.enabled = false;
        yield return new WaitForSeconds(2f);
        lives--;
        playerInfo.lives = lives;
        SerializeJson();
        infoUI.SetActive(false);
        rb.gravityScale = 0;
        GetComponent<BoxCollider2D>().enabled = false;
        if(lives > 0){
            deathScreen.SetActive(true);
        }else{
            gameoverScreen.SetActive(true);
            playerInfo.lives = 5;
            SerializeJson();
        }
        // Scene scene = SceneManager.GetActiveScene();
        // SceneManager.LoadScene(scene.name);
        // yield return null;
    }
    bool IsGrounded() {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 1.0f;
        
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null) {
            return true;
        }
        
        return false;
    }
    private void checks(){
        //check for idle
        if(rb.velocity == Vector2.zero){
            animator.SetBool("isIdle", true);
        }
        else{
            animator.SetBool("isIdle", false);
        }
        //check for is falling
        if(rb.velocity.y < 0 && !IsGrounded()){
            animator.SetBool("isFalling", true);
        }else{
            animator.SetBool("isFalling", false);
        }
        //Reset jump amount
        if(IsGrounded()){
            jumpCount = jumpCountMax;
        }
    }
    private void UIUpdate(){

        if(health > maxHealth){
            health = maxHealth;
        }
        for(int i = 0; i < hearts.Length; i++){

            if(i < health){
                hearts[i].sprite = fullHeart;
            }else{
                hearts[i].sprite = emptyHeart;
            }
            if(i < maxHealth){
                hearts[i].enabled = true;
            }else{
                hearts[i].enabled = false;
            }
        }
        livesText.text = "x " + lives;
    }
    public void SerializeJson(){
        DataService.SaveData("/player-stats.json", playerInfo, false);
    }
    public void LoadJson(){
        string path = Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + ".json";
        if(File.Exists(path)){
            PlayerInfo data = DataService.LoadData<PlayerInfo>("/player-stats.json", false);
            lives = data.lives;
        }

    }
}
