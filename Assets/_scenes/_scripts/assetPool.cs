using UnityEngine;
using System.Collections;

public class assetPool : MonoBehaviour {

    public static assetPool instance;

    public GameObject castle;

    public GameObject swordsman;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }
}
