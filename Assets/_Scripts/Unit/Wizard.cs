namespace Unit {

    using System;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEditor;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using UI;
    using Utility;

    [RequireComponent(typeof(WizardUI))]
    public sealed class Wizard : UnitBase {

        #region VARIABLE
        [Header("WIZARD")]
        [SerializeField, Range(1.0f, 50.0f)] private float _splashRadius = 10.0f;

        public float SplashRadius { get { return this._splashRadius; } }
        #endregion

        #region CLASS_CLEAN

        #endregion

        #region CLASS

        protected override void InternalAttack(float damage) {
            this.SpawnAttackParticle();

            this._currentTarget.LastAttacker = this;
            this._currentTarget.ReceiveDamage(damage, this as IHasHealth, this.transform.position);

            // NOTE: Find units within the splashRadius and calculate damage on the distance from the main attack source.
            var hits = Utils.hitsBuffers;
            int numberofhits = Physics.SphereCastNonAlloc(this.CurrentTarget.position, this._splashRadius + this._unitRadius, Vector3.forward, hits, 0.0f, GlobalSettings.LayerValues.unitLayer | GlobalSettings.LayerValues.structureLayer);
            this._hitComparer.position = this._currentTarget.position;
            Array.Sort(hits, this._hitComparer);

            Debug.Log("Start Splash Attack To: " + numberofhits.ToString() + " Enemies");

            List<IHasHealth> unitsToAttack = new List<IHasHealth>();

            for(int i = 0; i < hits.Length; i++) {
                var hit = hits[i];

                if(hit.transform == null)
                    continue;

                if(hit.transform == this.transform)
                    continue;

                if(hit.transform == this._currentTarget.transform)
                    continue;

                var hitHasHealth = hit.collider.GetEntity<IHasHealth>();
                if(hitHasHealth == null || hitHasHealth.isDead)
                    continue;

                if(this.IsAlly(hitHasHealth))
                    continue;

                unitsToAttack.Add(hitHasHealth);

                float distance = Vector3.Distance(this._currentTarget.position, hitHasHealth.position);
                float finalDamage = 0.0f;
                finalDamage = Mathf.Round(damage - ((distance / this._splashRadius) * damage));

                hitHasHealth.LastAttacker = this;
                hitHasHealth.ReceiveDamage(finalDamage, this as IHasHealth, this._currentTarget.position);
                //Debug.Log("Current Target (" + hitHasHealth.gameObject.name + "): Took " + finalDamage.ToString() + " of Damage");
            }

            this._currentState = UnitState.IDLE;
            this._canAttack = false;
        }

        protected override void SpawnAttackParticle() {
            ParticlePoolManager.instance.SpawnParticleSystem(ParticleType.IMPACT_WIZARD_ATTACK, this._currentTarget.position);
        }
        #endregion
    }
}