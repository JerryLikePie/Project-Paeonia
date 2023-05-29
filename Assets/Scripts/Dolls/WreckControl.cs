using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckControl : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= 10)
        {
            rb.velocity = Vector3.zero;
        }
    }
}
