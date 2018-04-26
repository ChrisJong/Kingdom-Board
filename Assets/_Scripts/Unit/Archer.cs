namespace Unit {

    using UnityEngine;

    using Constants;
    using Enum;
    using UI;

    [RequireComponent(typeof(ArcherUI))]
    public sealed class Archer : UnitBase {

        public override UnitType unitType { get { return UnitType.ARCHER; } }

        public override MovementType movementType { get { return MovementType.GROUND; } }

        public override AttackType attackType { get { return AttackType.PROJECTFILE; } }
        public override AttackType resistance { get { return AttackType.PHYSICAL; } }
        public override AttackType weakness { get { return AttackType.MAGIC; } }

        protected override void OnEnable() {
            base.OnEnable();

            this.currentHealth = UnitValues.ArcherValues.HEALTH;
            this._maxHealth = UnitValues.ArcherValues.HEALTH;
            this.currentEnergy = UnitValues.ArcherValues.ENERGY;
            this._maxEnergy = UnitValues.ArcherValues.ENERGY;

            this._moveSpeed = UnitValues.ArcherValues.MOVESPEED;
            this._moveRadius = UnitValues.ArcherValues.MOVERADIUS;

            this._minDamage = UnitValues.ArcherValues.MINDAMAGE;
            this._maxDamage = UnitValues.ArcherValues.MAXDAMAGE;
            this._attackRadius = UnitValues.ArcherValues.ATTACKRADIUS;

            this._resistancePercentage = UnitValues.ArcherValues.RESISTANCEPERCENTAGE;
            this._weaknessPercentage = UnitValues.ArcherValues.WEAKNESSPERCENTAGE;
        }

        protected override void InternalAttack(float damage) {
            base.InternalAttack(damage);

            //NOTE: emit a particle at the end of the attack.
        }
    }
}