namespace Unit {

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using UI;

    [RequireComponent(typeof(MageUI))]
    public sealed class Mage : UnitBase {

        private float _splashRadius;

        public float splashRadius { get { return this._splashRadius; } }

        public override UnitType unitType { get { return UnitType.MAGE; } }

        public override MovementType movementType { get { return MovementType.GROUND; } }

        public override AttackType attackType { get { return AttackType.MAGIC; } }
        public override AttackType resistance { get { return AttackType.PROJECTFILE; } }
        public override AttackType weakness { get { return AttackType.PHYSICAL; } }

        protected override void OnEnable() {

            this._splashRadius = UnitValues.MageValues.SPLASHRADIUS;

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

            base.OnEnable();
        }

        protected override void InternalAttack(float damage, IHasHealth target) {
            base.InternalAttack(damage, target);

            // NOTE: Find units within the splashRadius and calculate damage on the distance from the main attack source.
        }

    }
}