namespace Scriptable {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;

    [CreateAssetMenu(menuName = "Upgrades/Create Upgrade")]
    public class UpgradeScriptable : ScriptableObject {

        [Header("TYPE")]
        public ClassType classType = ClassType.NONE;
        public List<UnitType> unitTypes = new List<UnitType>();

        [Header("UPGRADE TYPE")]
        public UnitUpgradeType upgradeType = UnitUpgradeType.NONE;
        public float value = 1.0f;

        [Header("UI")]
        public Sprite cardFaceSprite = null;
        public Sprite cardBackSprite = null;
    }
}