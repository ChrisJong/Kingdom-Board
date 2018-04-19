namespace Structure {

    using UnityEngine;

    using Helpers;

    public sealed class StructurePool : PoolBase<IStructure> {

        public StructurePool(GameObject prefab, GameObject host, int initialInstanceCount) : base(prefab, host, initialInstanceCount) { }
    }
}