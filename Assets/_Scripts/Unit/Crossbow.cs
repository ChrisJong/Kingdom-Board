namespace Unit {

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using UI;
    using System;

    [RequireComponent(typeof(CrossbowUI))]
    public sealed class Crossbow : UnitBase {

        public override UnitType unitType { get { return UnitType.CROSSBOW; } }

        public override MovementType movementType { get { return UnitValues.Crossbow.MOVETYPE; } }

        public override AttackType attackType { get { return UnitValues.Crossbow.ATTACKTYPE; } }
        public override AttackType resistanceType { get { return UnitValues.Crossbow.RESISTANCETYPE; } }
        public override AttackType weaknessType { get { return UnitValues.Crossbow.WEAKNESSTYPE; } }

        protected override void OnEnable() {
            this.currentHealth = UnitValues.Crossbow.HEALTH;
            this._maxHealth = UnitValues.Crossbow.HEALTH;
            this.currentEnergy = UnitValues.Crossbow.ENERGY;
            this._maxEnergy = UnitValues.Crossbow.ENERGY;

            this._moveSpeed = UnitValues.Crossbow.MOVESPEED;
            this._moveRadius = UnitValues.Crossbow.MOVERADIUS;

            this._minDamage = UnitValues.Crossbow.MINDAMAGE;
            this._maxDamage = UnitValues.Crossbow.MAXDAMAGE;
            this._attackRadius = UnitValues.Crossbow.ATTACKRADIUS;

            this._resistancePercentage = UnitValues.Crossbow.RESISTANCEPERCENTAGE;
            this._weaknessPercentage = UnitValues.Crossbow.WEAKNESSPERCENTAGE;

            base.OnEnable();
        }
    }
}