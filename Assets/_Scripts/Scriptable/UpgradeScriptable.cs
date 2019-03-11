namespace KingdomBoard.Scriptable {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;

    [CreateAssetMenu(menuName = "Upgrades/Create Upgrade")]
    public class UpgradeScriptable : ScriptableObject {

        [Header("TYPE")]
        public UnitClassType classType = UnitClassType.NONE;
        public List<UnitType> unitTypes = new List<UnitType>();

        [Header("UPGRADE TYPE")]
        public UnitUpgradeType upgradeType = UnitUpgradeType.NONE;
        public float value = 1.0f;

        [Header("UI")]
        public Sprite researchCardFaceSprite = null;
        public Sprite researchCardBackSprite = null;
    }
}