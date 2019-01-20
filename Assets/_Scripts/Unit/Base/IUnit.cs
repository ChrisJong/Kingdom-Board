namespace Unit {

    using UnityEngine;

    using Enum;
    using Helpers;
    using Scriptable;
    
    public interface IUnit : IHasHealth, ICanMove, ICanAttack, ISelected {

        UnitScriptable Data { get; }
        UnitClassType classType { get; }
        UnitType unitType { get; }
        LayerMask AreaMask { get; }

        void NewTurn();
        void Finished();
    }
}