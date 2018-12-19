namespace Manager {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;
    using Extension;
    using Particle;

    /// <summary>
    /// Manager for handling particle systems and their pools. This manager is a singleton and will ensure that it is the only one in the scene.
    /// </summary>
    public class ParticlePoolManager : SingletonMono<ParticlePoolManager> {

        private static readonly int particleSystemLength = Enum.GetNames(typeof(ParticleType)).Length - 2;

        [SerializeField]
        private ParticlePoolSetup[] _poolSetup = new ParticlePoolSetup[particleSystemLength];

        private readonly Dictionary<ParticleType, ParticlePool> _pools = new Dictionary<ParticleType, ParticlePool>(particleSystemLength, new ParticleTypeComparer());

        protected override void Awake() {
            base.Awake();

            GameObject managerHost = new GameObject("Particles");
            managerHost.transform.SetParent(this.transform);

            for(int i = 0; i < this._poolSetup.Length; i++) {
                ParticlePoolSetup setup = this._poolSetup[i];

                if(setup.prefab.GetComponent<ParticleComponent>() == null)
                    setup.prefab.AddComponent<ParticleComponent>();

                GameObject host = new GameObject(setup.type.ToString());
                host.transform.SetParent(managerHost.transform);

                this._pools.Add(setup.type, new ParticlePool(setup.prefab, host, setup.initialInstanceCount));
            }
        }

        public override void Init() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Spawns a pariticle system of the given type at the specified position, with default rotation.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position"></param>
        public void SpawnParticleSystem(ParticleType type, Vector3 position) {
            this.SpawnParticleSystem(type, position, Quaternion.identity);
        }

        /// <summary>
        /// Spawns a particle system of the given type at the specified position and rotation.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void SpawnParticleSystem(ParticleType type, Vector3 position, Quaternion rotation) {
            IParticleSystem system = this._pools[type].Get(position, rotation);

            if(system == null)
                Debug.LogError("System NOt Found");

            system.Play();

            if(this.gameObject.activeSelf)
                StartCoroutine(this.ReturnParticleSystem(type, system));
        }

        /// <summary>
        /// Returns the given particle system to the pool from where it came from.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="system"></param>
        /// <returns></returns>
        private IEnumerator ReturnParticleSystem(ParticleType type, IParticleSystem system) {
            yield return new WaitForSeconds(system.duration);
            this._pools[type].Return(system);
        } 
    }
}