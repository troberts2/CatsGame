using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;

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
    private int dashCount = 2;
    private int dashCountMax = 2;
    //Player status
    private int health = 5;
    private bool iFrames = false;

    //Input actions
    public PlayerInputActions playerControls;
    private Vector2 moveDir = Vector2.zero;
    private InputAction move;
    private InputAction jump;
    private InputAction dash;
    private InputAction pencil;
    private InputAction sprinkle;
    private InputAction squigle;

    //UI
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI scoreText;

    //other
    private Animator animator;

    void Awake(){
        playerControls = new PlayerInputActions();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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
    }

    /// <summary>
    /// currently just moves the player
    /// </summary>
    private void FixedUpdate() {
        //basic movement with addforce
        rb.AddForce(new Vector2(moveDir.x * moveSpeed, 0), ForceMode2D.Force);
        //check for idle
        if(rb.velocity == Vector2.zero){
            animator.SetBool("isIdle", true);
        }
        else{
            animator.SetBool("isIdle", false);
        }
        //check for is falling
        if(rb.velocity.y < 0){
            animator.SetBool("isFalling", true);
        }else{
            animator.SetBool("isFalling", false);
        }
    }
    private void MovePerformed(InputAction.CallbackContext callbackContext){
        moveDir = callbackContext.ReadValue<Vector2>();
        if(moveDir.x > 0){
            transform.localScale = Vector2.one;
        }else{
            transform.localScale = new Vector2(-1f, 1f);
        }
        animator.SetBool("isRunning", true);
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
        if(jumpCount > 0){
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount--;
            animator.SetTrigger("jump");
        }
    }
    /// <summary>
    /// dash functionallity. Adds force as long as player is moving in a direction
    /// </summary>
    /// <param name="context"></param>
    private void Dash(InputAction.CallbackContext context){
        if(dashCount > 0 && moveDir != Vector2.zero){
            Debug.Log("dash");
            rb.AddForce(moveDir * dashForce, ForceMode2D.Impulse);
            dashCount--;
            animator.SetTrigger("dash");
            StartCoroutine(DashTimer());
            StartCoroutine(DashConstraint());
        }
    }
    private void Pencil(InputAction.CallbackContext callbackContext){
        animator.SetTrigger("pencilAttack");
    }
    private void Sprinkle(InputAction.CallbackContext callbackContext){
        animator.SetTrigger("sprinkleAttack");
    }
    private void Squigle(InputAction.CallbackContext callbackContext){
        animator.SetTrigger("squigleAttack");
    }
    /// <summary>
    /// resets the jump count
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.tag == "ground"){ 
            isGrounded = true;
            jumpCount = jumpCountMax;
        }
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
    }
    private void OnCollisionExit2D(Collision2D other) {
        if(other.collider.tag == "ground"){ isGrounded = false;}
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
        health--;
        healthText.text = "Health: " + health;
        iFrames = true;
        yield return new WaitForSeconds(.5f);
        iFrames = false;
    }
    private IEnumerator Die(){
        animator.SetTrigger("dead");
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        yield return null;
    }
}
