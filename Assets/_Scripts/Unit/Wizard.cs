namespace Unit {

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using UI;

    [RequireComponent(typeof(WizardUI))]
    public sealed class Wizard : UnitBase {

        private float _splashRadius;

        public float splashRadius { get { return this._splashRadius; } }

        public override UnitType unitType { get { return UnitType.WIZARD; } }

        public override MovementType movementType { get { return UnitValues.Wizard.MOVETYPE; } }

        public override AttackType attackType { get { return UnitValues.Wizard.ATTACKTYPE; } }
        public override AttackType resistanceType { get { return UnitValues.Wizard.RESISTANCETYPE; } }
        public override AttackType weaknessType { get { return UnitValues.Wizard.WEAKNESSTYPE; } }

        protected override void OnEnable() {

            this._splashRadius = UnitValues.Wizard.SPLASHRADIUS;

            this.currentHealth = UnitValues.Wizard.HEALTH;
            this._maxHealth = UnitValues.Wizard.HEALTH;
            this.currentEnergy = UnitValues.Wizard.ENERGY;
            this._maxEnergy = UnitValues.Wizard.ENERGY;

            this._moveSpeed = UnitValues.Wizard.MOVESPEED;
            this._moveRadius = UnitValues.Wizard.MOVERADIUS;

            this._minDamage = UnitValues.Wizard.MINDAMAGE;
            this._maxDamage = UnitValues.Wizard.MAXDAMAGE;
            this._attackRadius = UnitValues.Wizard.ATTACKRADIUS;

            this._resistancePercentage = UnitValues.Wizard.RESISTANCEPERCENTAGE;
            this._weaknessPercentage = UnitValues.Wizard.WEAKNESSPERCENTAGE;

            base.OnEnable();
        }

        protected override void InternalAttack(float damage, IHasHealth target) {
            base.InternalAttack(damage, target);

            // NOTE: Find units within the splashRadius and calculate damage on the distance from the main attack source.
        }
    }
}