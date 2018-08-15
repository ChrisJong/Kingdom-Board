namespace Unit {

    using System;

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using UI;
    using Utility;

    [RequireComponent(typeof(ClericUI))]
    public sealed class Cleric : UnitBase {

        #region 
        [Header("CLERIC - HEALING")]
        [SerializeField] private bool _canHeal = true;
        [SerializeField, Range(1.0f, 50.0f)] private float _healingRadius = 10.0f;
        [SerializeField, Range(1.0f, 100.0f)] private float _healingAmount = 5.0f;

        public bool canHeal { get { return this._canHeal; } }
        public float healingRadius { get { return this._healingRadius; } }
        public float healingAmoumt { get { return this._healingAmount; } }
        #endregion

        #region UNITY
        protected override void OnEnable() {
            this._canHeal = true;

            base.OnEnable();
        }
        #endregion

        #region CLASS
        public override void NewTurn() {
            base.NewTurn();
            this._canHeal = true;
        }

        /////////////////
        //// HEALING ////
        /////////////////
        public void FinishHealing() {
            this._canHeal = false;
        }

        public void Heal(IHasHealth target) {
            this.LookAt(target.position);
            
            this._unitAnimator.Play("Cast");

            this.InternalHeal(target);

            ((UI.ClericUI)this._uiComponent).FinishHeal();
            this._canHeal = false;
        }

        public void InternalHeal(IHasHealth target) {
            var hits = Utils.hitsBuffers;
            //var pos = this.position + this.transform.forward * this._unitRadius;
            var pos = target.position;
            Physics.SphereCastNonAlloc(pos, this._unitRadius * 2.0f, this.transform.forward, hits, this._healingRadius, GlobalSettings.LayerValues.unitLayer | GlobalSettings.LayerValues.structureLayer);

            this._hitComparer.position = this.position;
            Array.Sort(hits, this._hitComparer);

            for(int i = 0; i < hits.Length; i++) {
                var hit = hits[i];

                if(hit.transform == null)
                    continue;

                //if(hit.transform == this.transform)
                    //continue; // jgnore hits with itself;

                var hasHealth = hit.collider.GetEntity<IHasHealth>();
                if(hasHealth == null || hasHealth.isDead)
                    continue; // ignore anything that doesn't contain health or is dead.

                if(this.IsEnemy(hasHealth))
                    continue; // ignore enemies.

                // only attack the original target chosen.
                hasHealth.AddHealth(this._healingAmount);
                break;
            }
        }
        #endregion
    }
}