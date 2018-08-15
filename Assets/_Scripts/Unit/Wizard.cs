namespace Unit {

    using System;

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using UI;
    using Utility;

    [RequireComponent(typeof(WizardUI))]
    public sealed class Wizard : UnitBase {

        #region VARIABLE
        [Header("WIZARD - ATTACK")]
        [SerializeField, Range(1.0f, 50.0f)] private float _splashRadius = 10.0f;
        public float splashRadius { get { return this._splashRadius; } }
        #endregion

        #region CLASS
        protected override void ATTACKSTANDBYSTATE() {
            base.ATTACKSTANDBYSTATE();

            Debug.Log("Current Aoe Coord: " + this.controller.playerSelection.GetCurrentPointOnGround().ToString());
        }

        protected override void InternalAttack(float damage) {
            // NOTE: Find units within the splashRadius and calculate damage on the distance from the main attack source.
            var hits = Utils.hitsBuffers;
            int numberofhits = Physics.SphereCastNonAlloc(this.currentTarget.position, this._splashRadius + this._unitRadius, Vector3.forward, hits, 0.0f, GlobalSettings.LayerValues.unitLayer | GlobalSettings.LayerValues.structureLayer);
            this._hitComparer.position = this._currentTarget.position;
            Array.Sort(hits, this._hitComparer);

            Debug.Log("Start Splash Attack To: " + numberofhits.ToString() + " Enemies");

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

                float distance = Vector3.Distance(this._currentTarget.position, hitHasHealth.position);
                float finalDamage = 0.0f;
                finalDamage = Mathf.Round(damage - ((distance / this._splashRadius) * damage));

                hitHasHealth.lastAttacker = this;
                hitHasHealth.ReceiveDamage(finalDamage, this as IHasHealth);
                //Debug.Log("Current Target (" + hitHasHealth.gameObject.name + "): Took " + finalDamage.ToString() + " of Damage");
            }
            base.InternalAttack(damage);
        }
        #endregion
    }
}