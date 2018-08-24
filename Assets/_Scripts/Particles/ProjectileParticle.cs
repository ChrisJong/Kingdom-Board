namespace Particle {

    using UnityEngine;

    public sealed class ProjectileParticle : MonoBehaviour {

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

        private void Awake() {
            if(this._particleSystem == null)
                this._particleSystem = this.GetComponent<ParticleSystem>() as ParticleSystem;
        }

        private void Update() {
            if(!this._particleSystem.IsAlive() || !this._particleSystem.isEmitting || this._particleSystem.time > (this.duration - 0.5f)) {
                Destroy(this.gameObject);
            }
        }
    }
}