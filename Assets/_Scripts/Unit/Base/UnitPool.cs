namespace Unit {

    using UnityEngine;

    using Helpers;

    public sealed class UnitPool : PoolBase<IUnit> {

        public UnitPool(GameObject prefab, GameObject host, int initialInstanceCount) : base (prefab, host, initialInstanceCount) {}
    }
}