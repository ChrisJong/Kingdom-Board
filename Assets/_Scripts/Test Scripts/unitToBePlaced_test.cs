using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitToBePlaced_test : MonoBehaviour
{
    private Renderer[] rends;

    private void Awake()
    {
        rends = GetComponentsInChildren<Renderer>();
    }

    public void ChangeColor(bool _valid)
    {
        Color color = (_valid) ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);

        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material.color = color;
        }
    }
}
