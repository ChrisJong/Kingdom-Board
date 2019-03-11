namespace KingdomBoard.Scriptable {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;

    [CreateAssetMenu(menuName = "Class/Create Class")]
    public class ClassScriptable : ScriptableObject {
        [Header("TYPES")]
        public UnitClassType classType = UnitClassType.NONE;

        [Header("UI")]
        public Sprite researchCardFaceSprite = null;
        public Sprite researchCardBackSprite = null;
        public Sprite iconSprite = null;
        public Color classColor = Color.white;
    }
}