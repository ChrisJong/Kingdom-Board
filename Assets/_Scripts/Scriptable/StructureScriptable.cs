namespace Structure {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;

    [CreateAssetMenu(menuName = "Structure/Create Structure")]
    public class StructureScriptable : ScriptableObject {

        [Header("TYPES")]
        public StructureClassType groupType = StructureClassType.NONE;
        public StructureClassType classType = StructureClassType.NONE;
        public StructureType structureType = StructureType.NONE;

        [Header("PREFAB")]
        public GameObject mainPrefab;

        [Header("PREFAB - SPAWN STRUCTURE")]
        public GameObject trainingButtonPrefab;
        public GameObject QueueButtonPrefab;

        [Header("DATA - ATTRIBUTE VALUES")]
        public float health = 0.0f;
        public float spawnRange = 0.0f;

        public int queueLimit = 0;
    }
}
