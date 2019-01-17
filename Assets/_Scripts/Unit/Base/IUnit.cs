namespace Unit {

    using UnityEngine;

    using Enum;
    using Helpers;
    using Scriptable;
    
    public interface IUnit : IHasHealth, ICanMove, ICanAttack, ISelected {

        UnitScriptable UnitData { get; set; }
        UnitClassType classType { get; }
        UnitType unitType { get; }
        LayerMask areaMask { get; }

        void NewTurn();
        void Finished();
    }
}