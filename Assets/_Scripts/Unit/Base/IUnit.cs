namespace Unit {

    using UnityEngine;

    using Enum;
    using Helpers;
    using Scriptable;
    
    public interface IUnit : IHasHealth, ICanMove, ICanAttack, ISelected {

        UnitScriptable Data { get; }
        UnitClassType classType { get; }
        UnitType unitType { get; }
        UnitState PreviousState { get; }
        UnitState CurrentState { get; set; }
        UnitState NextState { get; set; }
        LayerMask AreaMask { get; }

        UI.UnitUI unitUI { get; }

        void NewTurn();
        void Finished();
    }
}