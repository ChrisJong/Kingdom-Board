namespace Unit {

    using UnityEngine;

    using Enum;
    using Helpers;
    
    public interface IUnit : IHasHealth, ICanMove, ICanAttack {
        UnitType unitType { get; }
        LayerMask areaMask { get; }

        void NewTurn();
        void Finished();
    }
}