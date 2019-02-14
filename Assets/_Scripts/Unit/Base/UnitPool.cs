namespace Unit {

    using UnityEngine;

    using Helpers;

    public sealed class UnitPool : PoolBase<IUnit> {

        public UnitPool(GameObject prefab, GameObject host, int initialInstanceCount) : base (prefab, host, initialInstanceCount) {}
    }

    public sealed class UnitDeathPool : PoolBase<IUnitDeath> {
        public UnitDeathPool(GameObject prefab, GameObject host, int initialInstanceCount) : base(prefab, host, initialInstanceCount) { }
    }

    public class UnitPlacementPool : PoolBase<IUnitPlacement> {

        public UnitPlacementPool(GameObject prefab, GameObject host, int initalInstanceCount) : base(prefab, host, initalInstanceCount) { }
    }
}