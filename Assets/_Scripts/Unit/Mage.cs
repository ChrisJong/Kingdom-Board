namespace Unit {

    using System;

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using UI;
    using Utility;

    [RequireComponent(typeof(MageUI))]
    public sealed class Mage : UnitBase {

        private float _splashRadius;

        public float splashRadius { get { return this._splashRadius; } }

        public override UnitType unitType { get { return UnitType.MAGE; } }

        public override MovementType movementType { get { return UnitValues.Mage.MOVETYPE; } }

        public override AttackType attackType { get { return UnitValues.Mage.ATTACKTYPE; } }
        public override AttackType resistanceType { get { return UnitValues.Mage.RESISTANCETYPE; } }
        public override AttackType weaknessType { get { return UnitValues.Mage.WEAKNESSTYPE; } }

        protected override void OnEnable() {

            this._splashRadius = UnitValues.Mage.SPLASHRADIUS;

            this.currentHealth = UnitValues.Mage.HEALTH;
            this._maxHealth = UnitValues.Mage.HEALTH;
            this.currentEnergy = UnitValues.Mage.ENERGY;
            this._maxEnergy = UnitValues.Mage.ENERGY;

            this._moveSpeed = UnitValues.Mage.MOVESPEED;
            this._moveRadius = UnitValues.Mage.MOVERADIUS;

            this._minDamage = UnitValues.Mage.MINDAMAGE;
            this._maxDamage = UnitValues.Mage.MAXDAMAGE;
            this._attackRadius = UnitValues.Mage.ATTACKRADIUS;

            this._resistancePercentage = UnitValues.Mage.RESISTANCEPERCENTAGE;
            this._weaknessPercentage = UnitValues.Mage.WEAKNESSPERCENTAGE;

            base.OnEnable();
        }

        protected override void InternalAttack(float damage, IHasHealth target) {
            var hits = Utils.hitsBuffers;
            var pos = target.position;

            Physics.SphereCastNonAlloc(pos, this.unitRadius * 2.0f, this.transform.forward, hits, this._splashRadius, GlobalSettings.LayerValues.unitLayer | GlobalSettings.LayerValues.structureLayer);

            this._hitComparer.position = pos;
            Array.Sort(hits, this._hitComparer);

            for(int i = 0; i < hits.Length; i++) {
                var hit = hits[i];

                if(hit.transform == null)
                    continue;

                var hasHealth = hit.collider.GetEntity<IHasHealth>();
                if(hasHealth == null || hasHealth.isDead)
                    continue;

                if(this.IsAlly(hasHealth))
                    continue;

                Debug.Log(hasHealth.gameObject.name + " - " + hasHealth.controller.name);

                if(hasHealth.Equals(target)) {
                    hasHealth.lastAttacker = this;
                    hasHealth.ReceiveDamage(damage, this as IHasHealth);
                    continue;
                }

                float distance = Vector3.Distance(target.position, hasHealth.position);
                Debug.Log(distance.ToString());
                double roundTo = Math.Round(((double)(distance / this._splashRadius)), 1);
                float splashDamange = damage * (1.0f - (float)roundTo);
                Debug.Log("Splash: " + damage);

                hasHealth.lastAttacker = this;
                hasHealth.ReceiveDamage(splashDamange, this as IHasHealth);
            }
        }
    }
}