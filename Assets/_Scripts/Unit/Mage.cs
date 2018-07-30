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

        [Header("ATTACK")]
        [SerializeField, Range(1.0f, 50.0f)]
        private float _splashRadius = 10.0f;
        public float splashRadius { get { return this._splashRadius; } }

        protected override void InternalAttack(float damage) {
            var hits = Utils.hitsBuffers;
            var pos = this._currentTarget.position;

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

                if(hasHealth.Equals(this._currentTarget)) {
                    hasHealth.lastAttacker = this;
                    hasHealth.ReceiveDamage(damage, this as IHasHealth);
                    continue;
                }

                float distance = Vector3.Distance(this._currentTarget.position, hasHealth.position);
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