namespace Scriptable {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;

    [CreateAssetMenu(menuName = "Class/Create Class")]
    public class ClassScriptable : ScriptableObject {
        [Header("TYPES")]
        public ClassType classType = ClassType.NONE;

        [Header("UI")]
        public Sprite cardFaceSprite = null;
        public Sprite cardBackSprite = null;
        public Sprite Icon = null;
    }
}