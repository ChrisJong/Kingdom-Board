namespace Decal {

    using UnityEngine;

    using Enum;
    using Helpers;

    public sealed class DecalPool : PoolBase<IDecal> {

        public DecalPool(GameObject prefab, GameObject host, int initialInstanceCount) : base(prefab, host, initialInstanceCount) { }
    }
}