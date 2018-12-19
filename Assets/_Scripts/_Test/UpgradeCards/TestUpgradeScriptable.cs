namespace Test {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;

    //[CreateAssetMenu(menuName = "Upgrade Generator/Create Upgrades")]
    public class TestUpgradeScriptable : ScriptableObject {

        [Header("TYPE")]
        public ClassType classType = ClassType.NONE;
        public List<UnitType> unitType = new List<UnitType>();

        [Header("UPGRADE")]
        public UnitUpgrade upgrade = UnitUpgrade.NONE;
        public float value = 0.1f;

        [Header("UI")]
        public Sprite cardFront;
        public Sprite cardBack;
    }
}