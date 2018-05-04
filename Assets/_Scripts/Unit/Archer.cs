namespace Unit {

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using UI;

    [RequireComponent(typeof(ArcherUI))]
    public sealed class Archer : UnitBase {

        public override UnitType unitType { get { return UnitType.ARCHER; } }

        public override MovementType movementType { get { return UnitValues.Archer.MOVETYPE; } }

        public override AttackType attackType { get { return UnitValues.Archer.ATTACKTYPE; } }
        public override AttackType resistanceType { get { return UnitValues.Archer.RESISTANCETYPE; } }
        public override AttackType weaknessType { get { return UnitValues.Archer.WEAKNESSTYPE; } }

        protected override void OnEnable() {

            this.currentHealth = UnitValues.Archer.HEALTH;
            this._maxHealth = UnitValues.Archer.HEALTH;
            this.currentEnergy = UnitValues.Archer.ENERGY;
            this._maxEnergy = UnitValues.Archer.ENERGY;

            this._moveSpeed = UnitValues.Archer.MOVESPEED;
            this._moveRadius = UnitValues.Archer.MOVERADIUS;

            this._minDamage = UnitValues.Archer.MINDAMAGE;
            this._maxDamage = UnitValues.Archer.MAXDAMAGE;
            this._attackRadius = UnitValues.Archer.ATTACKRADIUS;

            this._resistancePercentage = UnitValues.Archer.RESISTANCEPERCENTAGE;
            this._weaknessPercentage = UnitValues.Archer.WEAKNESSPERCENTAGE;

            base.OnEnable();
        }

        protected override void InternalAttack(float damage, IHasHealth target) {
            base.InternalAttack(damage, target);

            //NOTE: emit a particle at the end of the attack.
        }
    }
}