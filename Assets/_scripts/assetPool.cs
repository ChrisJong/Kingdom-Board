using UnityEngine;
using System.Collections;

public class AssetPool : Extension.SingletonMono<AssetPool> {

    public GameObject mainManager;
    public GameObject gameManager;
    public GameObject PlayerBJ;

    #region STRUCTURE
    public GameObject castle;
    #endregion

    #region UNITS
    public GameObject warriorOBJ;
    public GameObject archerOBH;
    public GameObject magicianOBJ;
    #endregion
}
