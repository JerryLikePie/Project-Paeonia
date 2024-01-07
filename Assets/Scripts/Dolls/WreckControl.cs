using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckControl : MonoBehaviour
{
    Rigidbody rb;
    bool exploded;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        exploded = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= 10 && exploded == false)
        {
            // on the ground, stops it
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            // and then explode
            exploded = true;
            explode();
        }
    }

    void explode()
    {
        // play the explode animation, swap the sprite to the exploded version
    }
}
