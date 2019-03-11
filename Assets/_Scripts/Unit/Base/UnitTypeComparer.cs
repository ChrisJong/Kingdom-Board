namespace KingdomBoard.Unit {

    using System.Collections.Generic;

    using Enum;

    public class UnitTypeComparer : IEqualityComparer<UnitType> {

        public bool Equals(UnitType x, UnitType y) {
            return x == y;
        }

        public int GetHashCode(UnitType obj) {
            return (int)obj;
        }
    }
}