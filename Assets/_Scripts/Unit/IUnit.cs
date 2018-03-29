namespace Unit {

    using Enum;
    using Helpers;

    using UnityEngine;
    
    public interface IUnit : IHasHealth, ICanMove, ICanAttack {
        UnitType unitType { get; }
        LayerMask areaMask { get; }
    }
}