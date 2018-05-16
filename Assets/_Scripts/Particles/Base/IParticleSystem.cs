namespace Particle {
 
    using Helpers;

    /// <summary>
    /// interface representing particle system componenets that are instance pooled.
    /// </summary>
    public interface IParticleSystem : IObjectPool {
        /// <summary>
        /// Gets the duration of the pariicle system, as set on particle system.
        /// </summary>
        float duration { get; }

        /// <summary>
        /// Plays the particle system.
        /// </summary>
        void Play();
    }
}