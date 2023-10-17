using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class KeithEnemyMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed;

    private void FixedUpdate(){
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
    }
}
