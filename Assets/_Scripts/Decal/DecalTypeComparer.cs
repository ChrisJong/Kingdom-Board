namespace KingdomBoard.Decal {

    using System.Collections;
    using System.Collections.Generic;

    using Enum;

    public sealed class DecalTypeComprarer : IEqualityComparer<DecalType> {

        public bool Equals(DecalType x, DecalType y) {
            return x == y;
        }

        public int GetHashCode(DecalType obj) {
            return (int)obj;
        }
    }
}