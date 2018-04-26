namespace Unit {

    using System;

    using UnityEngine;

    using Constants;
    using Enum;
    using UI;

    [RequireComponent(typeof(WarriorUI))]
    public sealed class Warrior : UnitBase {

        public override UnitType unitType { get { return UnitType.WARRIOR; } }

        public override MovementType movementType { get { return UnitValues.WarriorValues.MOVETYPE; } }

        public override AttackType attackType { get { return UnitValues.WarriorValues.ATTACKTYPE; } }
        public override AttackType resistance { get { return UnitValues.WarriorValues.RESISTANCE; } }
        public override AttackType weakness { get { return UnitValues.WarriorValues.WEAKNESS; } }

        protected override void OnEnable() {
            base.OnEnable();

            this.currentHealth = UnitValues.WarriorValues.HEALTH;
            this._maxHealth = UnitValues.WarriorValues.HEALTH;
            this.currentEnergy = UnitValues.WarriorValues.ENERGY;
            this._maxEnergy = UnitValues.WarriorValues.ENERGY;

            this._moveSpeed = UnitValues.WarriorValues.MOVESPEED;
            this._moveRadius = UnitValues.WarriorValues.MOVERADIUS;

            this._minDamage = UnitValues.WarriorValues.MINDAMAGE;
            this._maxDamage = UnitValues.WarriorValues.MAXDAMAGE;
            this._attackRadius = UnitValues.WarriorValues.ATTACKRADIUS;

            this._resistancePercentage = UnitValues.WarriorValues.RESISTANCEPERCENTAGE;
            this._weaknessPercentage = UnitValues.WarriorValues.WEAKNESSPERCENTAGE;
        }
    }
}