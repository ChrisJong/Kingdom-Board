using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectionMaterialSwapTest_dan : MonoBehaviour {

    Renderer[] rs;

    private void Start()
    {
        rs = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        //Control outline colour
        //O for green, P for red
        if (Input.GetKeyDown("o"))
        {
            foreach (Renderer r in rs)
            {
                r.material.SetColor("_OutlineColor", Color.green);
            }
        }

        if (Input.GetKeyDown("p"))
        {
            foreach (Renderer r in rs)
            {
                r.material.SetColor("_OutlineColor", Color.red);
            }
        }
        //Control show outline
        //K to hide, L to show
        if (Input.GetKeyDown("k"))
        {
            foreach (Renderer r in rs) {
                r.material.SetFloat("_Outline", 0f);
            }
        }

        if (Input.GetKeyDown("l"))
        {
            foreach (Renderer r in rs)
            {
                r.material.SetFloat("_Outline", 0.03f);
            }
        }
    }
}
