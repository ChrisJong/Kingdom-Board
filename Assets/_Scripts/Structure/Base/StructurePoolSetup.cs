namespace Structure {

    using System;

    using UnityEngine;

    using Enum;

    [Serializable]
    public sealed class StructurePoolSetup {
        public StructureType type;
        public GameObject prefab;

        [Range(1, 100)]
        public int initialInstanceCount = 5;
    }
}