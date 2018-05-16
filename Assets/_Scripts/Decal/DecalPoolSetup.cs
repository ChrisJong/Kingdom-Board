namespace Decal {

    using System;

    using UnityEngine;

    using Enum;

    [Serializable]
    public sealed class DecalPoolSetup {

        public DecalType type;
        public GameObject prefab;

        [Range(1, 100)]
        public int initialInstanceCount = 25;
    }
}