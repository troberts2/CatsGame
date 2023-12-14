using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poop : MonoBehaviour
{
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.CompareTag("ground")){
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}
