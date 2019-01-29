namespace Unit {

    using Helpers;

    public class UnitPlacementPool : PoolBase<IUnitPlacement> {

        public UnitPlacementPool(UnityEngine.GameObject prefab, UnityEngine.GameObject host, int initalInstanceCount) : base(prefab, host, initalInstanceCount) { }
    }
}