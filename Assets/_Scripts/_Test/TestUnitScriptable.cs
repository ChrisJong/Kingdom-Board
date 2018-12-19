namespace Test {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;

    //[CreateAssetMenu(menuName = "Unit Generator/Create Unit")]
    public class TestUnitScriptable : ScriptableObject {

        #region VARIABLE
        public string description;

        public UnitType unitType;
        public ClassType classType;

        public int tierLevel = 0;

        [Header("Unit - UI")]
        public Sprite cardFace;
        public Sprite cardBack;
        public Sprite spawnIcon;
        #endregion
    }
}