namespace KingdomBoard.Particle {

    using System;

    using UnityEngine;

    using Enum;

    /// <summary>
    /// Represents a "setup object" for pooled particle systems. used for easy setup and tweaking of pooled particle system prefabs.
    /// </summary>
    [Serializable]
    public sealed class ParticlePoolSetup {

        public ParticleType type;
        public GameObject prefab;

        [Range(1, 100)]
        public int initialInstanceCount = 3;
    }
}