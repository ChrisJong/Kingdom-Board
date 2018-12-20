namespace Unit {

    using UnityEngine;

    using Enum;
    using Helpers;
    
    public interface IUnit : IHasHealth, ICanMove, ICanAttack, ISelected {
        ClassType classType { get; }
        UnitType unitType { get; }
        LayerMask areaMask { get; }

        void NewTurn();
        void Finished();
    }
}