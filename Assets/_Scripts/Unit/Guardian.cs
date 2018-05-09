namespace Unit {

    using UnityEngine;

    using Constants;
    using Enum;
    using UI;

    [RequireComponent(typeof(GuardianUI))]
    public sealed class Guardian : UnitBase {

        public override UnitType unitType { get { return UnitType.GUARDIAN; } }

        public override MovementType movementType { get { return UnitValues.Guardian.MOVETYPE; } }

        public override AttackType attackType { get { return UnitValues.Guardian.ATTACKTYPE; } }
        public override AttackType resistanceType { get { return UnitValues.Guardian.RESISTANCETYPE; } }
        public override AttackType weaknessType { get { return UnitValues.Guardian.WEAKNESSTYPE; } }

        protected override void OnEnable() {

            this.currentHealth = UnitValues.Guardian.HEALTH;
            this._maxHealth = UnitValues.Guardian.HEALTH;
            this.currentEnergy = UnitValues.Guardian.ENERGY;
            this._maxEnergy = UnitValues.Guardian.ENERGY;

            this._moveSpeed = UnitValues.Guardian.MOVESPEED;
            this._moveRadius = UnitValues.Guardian.MOVERADIUS;

            this._minDamage = UnitValues.Guardian.MINDAMAGE;
            this._maxDamage = UnitValues.Guardian.MAXDAMAGE;
            this._attackRadius = UnitValues.Guardian.ATTACKRADIUS;

            this._resistancePercentage = UnitValues.Guardian.RESISTANCEPERCENTAGE;
            this._weaknessPercentage = UnitValues.Guardian.WEAKNESSPERCENTAGE;

            base.OnEnable();
        }
    }
}