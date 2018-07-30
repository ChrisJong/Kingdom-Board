namespace Unit {

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using UI;

    [RequireComponent(typeof(WizardUI))]
    public sealed class Wizard : UnitBase {

        [Header("ATTACK")]
        [SerializeField, Range(1.0f, 50.0f)]
        private float _splashRadius = 10.0f;
        public float splashRadius { get { return this._splashRadius; } }

        protected override void InternalAttack(float damage) {
            base.InternalAttack(damage);

            // NOTE: Find units within the splashRadius and calculate damage on the distance from the main attack source.
        }
    }
}