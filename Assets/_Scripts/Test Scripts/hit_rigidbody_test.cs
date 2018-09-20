using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hit_rigidbody_test : MonoBehaviour {

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnMouseDown()
    {
        Debug.Log("Click");
        rb.AddForce(-transform.forward * 100); 
    }

}
