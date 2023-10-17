using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
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

    void Awake(){
        playerControls = new PlayerInputActions();
    }

    private void OnEnable(){
        //sets up the playercontrols
        move = playerControls.Player.Move;
        move.Enable();

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
        jump.Disable();
        dash.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        moveDir = move.ReadValue<Vector2>();
    }
    /// <summary>
    /// currently just moves the player
    /// </summary>
    private void FixedUpdate() {
        //basic movement with addforce
        rb.AddForce(new Vector2(moveDir.x * moveSpeed, 0), ForceMode2D.Force);
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
