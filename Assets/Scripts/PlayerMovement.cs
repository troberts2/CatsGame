using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed;
    public float jumpForce;
    public float dashForce;
    private bool isGrounded;
    private int jumpCount;
    private int jumpCountMax = 2;
    private int dashCount = 2;
    private int dashCountMax = 2;
    public PlayerInputActions playerControls;
    Vector2 moveDir = Vector2.zero;
    private InputAction move;
    private InputAction jump;
    private InputAction dash;
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
    }

    private void OnDisable(){
        //disables player controls
        move.Disable();
        move.performed -= MovePerformed;
        move.canceled -= MoveCancelled;
        jump.Disable();
        dash.Disable();
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
    /// <summary>
    /// resets the jump count
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.tag == "ground"){ 
            isGrounded = true;
            jumpCount = jumpCountMax;
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
}
