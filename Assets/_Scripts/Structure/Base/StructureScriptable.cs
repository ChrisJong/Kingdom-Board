namespace Structure {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;

    [CreateAssetMenu(menuName = "Structure/Create Structure")]
    public class StructureScriptable : ScriptableObject {

        [Header("TYPES")]
        public StructureGroupType groupType = StructureGroupType.NONE;
        public StructureType structureType = StructureType.NONE;

        [Header("PREFAB")]
        public GameObject prefabMain;
        public GameObject prefabHealth;
        public GameObject prefabProjectile;

        [Header("UI")]
        public GameObject prefabSpawnButton;
        public GameObject prefabQueueButton;

        [Header("DATA - MAIN")]
        public float health = 0.0f;
        public float spawnRange = 0.0f;
    }
}
