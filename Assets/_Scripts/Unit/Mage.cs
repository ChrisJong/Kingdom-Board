namespace Unit {

    using UnityEngine;

    using Enum;
    using Manager;
    using UI;

    [RequireComponent(typeof(MageUI))]
    public sealed class Mage : UnitBase {

        #region CLASS
        protected override void SpawnAttackParticle() {
            ParticlePoolManager.instance.SpawnParticleSystem(ParticleType.IMPACT_MAGE_ATTACK, this._currentTarget.position);
        }
        #endregion
    }
}