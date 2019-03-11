namespace KingdomBoard.Particle {

    using UnityEngine;

    using Helpers;

    public sealed class ParticlePool : PoolBase<IParticleSystem> {

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticlePool"/> class. 
        /// </summary>
        /// <param name="prefab">the prefab from which to create the entity</param>
        /// <param name="host">the host that will be the parent of the entity instance</param>
        /// <param name="initialInstanceaCount">the initial instance count</param>
        public ParticlePool(GameObject prefab, GameObject host, int initialInstanceaCount) : base(prefab, host, initialInstanceaCount) { }
    }
}