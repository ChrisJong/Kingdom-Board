namespace Scriptable {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;

    [CreateAssetMenu(menuName = "Unit/Create Unit")]
    public class UnitScriptable : ScriptableObject {

        [Header("TYPES")]
        public ClassScriptable ClassData = null;
        public ClassType classType = ClassType.NONE;
        public UnitType unitType = UnitType.NONE;

        public int classTier = 0; // NOte: 0 is nothing. Start(base) is tier 1.

        [Header("UI")]
        public Sprite cardFaceSprite = null;
        public Sprite cardBackSprite = null;
        public Sprite TrainIconSprite = null;

        [Header("PREFAB")]
        public GameObject prefabMain;
        public GameObject prefabDeath;
        public GameObject prefabProjectile;

        [Header("DATA - ATTRIBUTE TYPES")]
        public MovementType movementType = MovementType.NONE;
        public AttackType attackType = AttackType.NONE;
        public AttackType resistanceType = AttackType.NONE;
        public AttackType weaknessType = AttackType.NONE;

        [Header("DATA - ATTRIBUTE VALUES")]
        public int turnCost = 1; // The Numbner of turns it takes for it to be able to spawn.
        public int goldCost = 500;
        public int populationCost = 1;
        [Range(0.0f, 3.0f)] public float resistanceMultiplier = 0.0f;
        [Range(0.0f, 3.0f)] public float weaknessMultiplier = 0.0f;
        public float heath = 0.0f;
        public float stamina = 0.0f;
        public float minAttack = 0.0f;
        public float maxAttack = 0.0f;
        public float attackRange = 1.5f; // MELEE = 1.5f, RANGE > 2.0f
        public float moveSpeed = 5.0f;

        [Header("DATA - ANIMATION")]
        public float projectileSpeed = 0.0f;
        public float endOfAttackClipTime = 0.0f;

        [Header("UNIQUE DATA - WIZARD")]
        public float splashRange = 0.0f;

        [Header("UNIQUE DATA - CLERIC")]
        public float healingDamage = 0.0f;
        public float healingRange = 0.0f;
        public float endOfCastClipTime = 0.0f; // for the healing animation
    }
}
