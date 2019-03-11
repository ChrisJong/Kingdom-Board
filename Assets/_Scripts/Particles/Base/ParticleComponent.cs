namespace KingdomBoard.Particle {

    using UnityEngine;

    using Helpers;

    /// <summary>
    /// Compnonent for the particle system. This component must be present on pooled particle systems.
    /// </summary>
    public sealed class ParticleComponent : ObjectPoolBase, IParticleSystem {

        private ParticleSystem _particleSystem;

        /// <summary>
        /// Gets the duration of the particle system, as set on the actual particle system asset.
        /// </summary>
        /// <valuie>
        /// The duration
        /// </valuie>
        public float duration {
            get { return this._particleSystem.main.duration; }
        }

        protected override void OnEnable() {
            base.OnEnable();

            this._particleSystem = this.GetComponent<ParticleSystem>() as ParticleSystem;
        }

        /// <summary>
        /// Play the particle system.
        /// </summary>
        public void Play() {
            this._particleSystem.Play();
        }
    }
}