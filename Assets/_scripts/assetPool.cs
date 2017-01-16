using UnityEngine;
using System.Collections;

public class assetPool : MonoBehaviour {

    public static assetPool instance;

    public GameObject mainManager;
    public GameObject gameManager;
    public GameObject PlayerObj;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }
}
