namespace Unit {

    using System;

    using UnityEngine;

    using Enum;

    [Serializable]
    public sealed class UnitPoolSetup {
        public UnitType type;
        public GameObject prefab;

        [Range(1, 100)] public int initialInstanceCount = 10;
    }
}