namespace Particle {

    using System.Collections.Generic;

    using Enum;

    /// <summary>
    /// Custom comparer for particle type enum to avoid memory allocations.
    /// </summary>
    public sealed class ParticleTypeComparer : IEqualityComparer<ParticleType> {

        public bool Equals(ParticleType x, ParticleType y) {
            return x == y;
        }

        public int GetHashCode(ParticleType obj) {
            return (int)obj;
        }
    }
}