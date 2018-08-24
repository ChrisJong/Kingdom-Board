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
        [Header("WIZARD - ATTACK")]
        [SerializeField, Range(1.0f, 50.0f)] private float _splashRadius = 10.0f;
        [SerializeField, ReadOnly] private LineRenderDrawCircle _splashRadiusDrawer = null;
        private Transform _splashRadiusTransform = null;

        public float splashRadius { get { return this._splashRadius; } }
        public LineRenderDrawCircle splashRadiusDrawer { get { return this._splashRadiusDrawer; } }
        #endregion

        #region UNITY
        protected override void Awake() {
            base.Awake();

            if(this._splashRadiusDrawer == null) {
                GameObject tempRadiusDrawer = new GameObject("SpashRadiusDrawer");
                tempRadiusDrawer.transform.position = this.position;

                LineRenderDrawCircle tempRadiusComp = tempRadiusDrawer.AddComponent<LineRenderDrawCircle>();
                tempRadiusComp.DrawAttackRadius(this._splashRadius);
                tempRadiusComp.TurnOff();

                tempRadiusDrawer.transform.SetParent(this.transform);

                this._splashRadiusDrawer = tempRadiusComp;
                this._splashRadiusTransform = tempRadiusDrawer.transform;
            }
        }
        #endregion

        #region CLASS

        protected override void ATTACKSTANDBYSTATE() {
            base.ATTACKSTANDBYSTATE();

            if(!this._splashRadiusDrawer.gameObject.activeSelf) {
                this._splashRadiusDrawer.TurnOn();

                // Debugging for changing the size of the splash radius at runtime/gametime.
                this._splashRadiusDrawer.UpdateRadius(this.splashRadius);
            } else {
                // Debugging for changing the size of the splash radius at runtime/gametime.
                this._splashRadiusDrawer.UpdateRadius(this.splashRadius);

                this._splashRadiusTransform.position = this.controller.playerSelection.GetCurrentPointOnGround();
            }

            //Debug.Log("Current Aoe Coord: " + this.controller.playerSelection.GetCurrentPointOnGround().ToString());
        }

        protected override void InternalAttack(float damage) {
            this.SpawnAttackParticle();

            this._currentTarget.lastAttacker = this;
            this._currentTarget.ReceiveDamage(damage, this as IHasHealth);

            // NOTE: Find units within the splashRadius and calculate damage on the distance from the main attack source.
            var hits = Utils.hitsBuffers;
            int numberofhits = Physics.SphereCastNonAlloc(this.currentTarget.position, this._splashRadius + this._unitRadius, Vector3.forward, hits, 0.0f, GlobalSettings.LayerValues.unitLayer | GlobalSettings.LayerValues.structureLayer);
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

                hitHasHealth.lastAttacker = this;
                hitHasHealth.ReceiveDamage(finalDamage, this as IHasHealth);
                //Debug.Log("Current Target (" + hitHasHealth.gameObject.name + "): Took " + finalDamage.ToString() + " of Damage");
            }

            ((UI.UnitUI)this._uiComponent).FinishAttack();
            this._unitState = UnitState.IDLE;
            this._canAttack = false;
        }

        protected override void SpawnAttackParticle() {
            ParticlePoolManager.instance.SpawnParticleSystem(ParticleType.IMPACT_WIZARD_ATTACK, this._currentTarget.position);
        }

        public override void StartStateAnimation() {
            this._splashRadiusDrawer.TurnOff();

            base.StartStateAnimation();
        }
        #endregion
    }
}