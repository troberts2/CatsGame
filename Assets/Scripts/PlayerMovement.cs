using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
        move.Disable();
        jump.Disable();
        dash.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        moveDir = move.ReadValue<Vector2>();
    }

    private void FixedUpdate() {
        rb.AddForce(new Vector2(moveDir.x * moveSpeed, 0), ForceMode2D.Force);
    }
    private void Jump(InputAction.CallbackContext context){
        if(jumpCount > 0){
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount--;
        }
    }

    private void Dash(InputAction.CallbackContext context){
        if(dashCount > 0){
            Debug.Log("dash");
            rb.AddForce(moveDir * dashForce, ForceMode2D.Impulse);
            dashCount--;
            StartCoroutine(DashTimer());
        }
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.tag == "ground"){ 
            isGrounded = true;
            jumpCount = jumpCountMax;
        }
    }
    private void OnCollisionExit2D(Collision2D other) {
        if(other.collider.tag == "ground"){ isGrounded = false;}
    }
    private IEnumerator DashTimer(){
        yield return new WaitForSeconds(2f);
        if(dashCount < dashCountMax){
            dashCount++;
        }
    }
}
