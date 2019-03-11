namespace KingdomBoard.Structure {

    using System.Collections.Generic;

    using Enum;

    public sealed class StructureTypeComparer : IEqualityComparer<StructureType> {

        public bool Equals(StructureType x, StructureType y) {
            return x == y;
        }

        public int GetHashCode(StructureType obj) {
            return (int)obj;
        }
    }
}