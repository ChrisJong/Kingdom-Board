using UnityEngine;
using System.Collections;

public class AssetPool : MonoBehaviour {

    public static AssetPool instance;

    public GameObject mainManager;
    public GameObject gameManager;
    public GameObject PlayerBJ;

    #region UNITS
    public GameObject warriorOBJ;
    public GameObject archerOBH;
    public GameObject magicianOBJ;
    #endregion

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }
}
