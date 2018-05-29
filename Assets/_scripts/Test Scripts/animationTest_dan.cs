using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationTest_dan : MonoBehaviour {

    private Animator anim;

    private void Start() {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("space")) {
            anim.Play("Attack");
        }           
	}
}
