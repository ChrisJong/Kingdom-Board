namespace Unit {

    using UnityEngine;

    using Constants;
    using Enum;
    using UI;

    [RequireComponent(typeof(WarriorUI))]
    public sealed class Warrior : UnitBase {

        public override UnitType unitType { get { return UnitType.WARRIOR; } }

        public override MovementType movementType { get { return UnitValues.Warrior.MOVETYPE; } }

        public override AttackType attackType { get { return UnitValues.Warrior.ATTACKTYPE; } }
        public override AttackType resistanceType { get { return UnitValues.Warrior.RESISTANCETYPE; } }
        public override AttackType weaknessType { get { return UnitValues.Warrior.WEAKNESSTYPE; } }

        protected override void OnEnable() {

            this.currentHealth = UnitValues.Warrior.HEALTH;
            this._maxHealth = UnitValues.Warrior.HEALTH;
            this.currentEnergy = UnitValues.Warrior.ENERGY;
            this._maxEnergy = UnitValues.Warrior.ENERGY;

            this._moveSpeed = UnitValues.Warrior.MOVESPEED;
            this._moveRadius = UnitValues.Warrior.MOVERADIUS;

            this._minDamage = UnitValues.Warrior.MINDAMAGE;
            this._maxDamage = UnitValues.Warrior.MAXDAMAGE;
            this._attackRadius = UnitValues.Warrior.ATTACKRADIUS;

            this._resistancePercentage = UnitValues.Warrior.RESISTANCEPERCENTAGE;
            this._weaknessPercentage = UnitValues.Warrior.WEAKNESSPERCENTAGE;

            base.OnEnable();
        }
    }
}