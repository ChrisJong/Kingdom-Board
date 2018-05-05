namespace Unit {

    using UnityEngine;

    using Constants;
    using Enum;
    using UI;

    [RequireComponent(typeof(WarriorUI))]
    public sealed class Knight : UnitBase {

        public override UnitType unitType { get { return UnitType.WARRIOR; } }

        public override MovementType movementType { get { return UnitValues.Knight.MOVETYPE; } }

        public override AttackType attackType { get { return UnitValues.Knight.ATTACKTYPE; } }
        public override AttackType resistanceType { get { return UnitValues.Knight.RESISTANCETYPE; } }
        public override AttackType weaknessType { get { return UnitValues.Knight.WEAKNESSTYPE; } }

        protected override void OnEnable() {

            this.currentHealth = UnitValues.Knight.HEALTH;
            this._maxHealth = UnitValues.Knight.HEALTH;
            this.currentEnergy = UnitValues.Knight.ENERGY;
            this._maxEnergy = UnitValues.Knight.ENERGY;

            this._moveSpeed = UnitValues.Knight.MOVESPEED;
            this._moveRadius = UnitValues.Knight.MOVERADIUS;

            this._minDamage = UnitValues.Knight.MINDAMAGE;
            this._maxDamage = UnitValues.Knight.MAXDAMAGE;
            this._attackRadius = UnitValues.Knight.ATTACKRADIUS;

            this._resistancePercentage = UnitValues.Knight.RESISTANCEPERCENTAGE;
            this._weaknessPercentage = UnitValues.Knight.WEAKNESSPERCENTAGE;

            base.OnEnable();
        }
    }
}