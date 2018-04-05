namespace Structure {

    using UnityEngine;

    using Helpers;

    public sealed class STructurePool : PoolBase<IStructure> {

        public STructurePool(GameObject prefab, GameObject host, int initialInstanceCount) : base(prefab, host, initialInstanceCount) { }
    }
}