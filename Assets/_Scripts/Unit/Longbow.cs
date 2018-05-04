namespace Unit {

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using UI;
    using System;

    [RequireComponent(typeof(LongbowUI))]
    public sealed class Longbow : UnitBase {

        public override UnitType unitType { get { return UnitType.LONGBOW; } }

        public override MovementType movementType { get { return UnitValues.Longbow.MOVETYPE; } }

        public override AttackType attackType { get { return UnitValues.Longbow.ATTACKTYPE; } }
        public override AttackType resistanceType { get { return UnitValues.Longbow.RESISTANCETYPE; } }
        public override AttackType weaknessType { get { return UnitValues.Longbow.WEAKNESSTYPE; } }

        protected override void OnEnable() {
            this.currentHealth = UnitValues.Longbow.HEALTH;
            this._maxHealth = UnitValues.Longbow.HEALTH;
            this.currentEnergy = UnitValues.Longbow.ENERGY;
            this._maxEnergy = UnitValues.Longbow.ENERGY;

            this._moveSpeed = UnitValues.Longbow.MOVESPEED;
            this._moveRadius = UnitValues.Longbow.MOVERADIUS;

            this._minDamage = UnitValues.Longbow.MINDAMAGE;
            this._maxDamage = UnitValues.Longbow.MAXDAMAGE;
            this._attackRadius = UnitValues.Longbow.ATTACKRADIUS;

            this._resistancePercentage = UnitValues.Longbow.RESISTANCEPERCENTAGE;
            this._weaknessPercentage = UnitValues.Longbow.WEAKNESSPERCENTAGE;

            base.OnEnable();
        }
    }
}