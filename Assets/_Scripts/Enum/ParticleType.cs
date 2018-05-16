namespace Enum {

    // NOTE: need new names for differernt types of particles/

    /// <summary>
    /// Enum Type for all pooled particles (Sizes: SMALL/MEDIUM/LARGE)
    /// </summary>
    public enum ParticleType {
        NONE = 0,
        ANY = ~0,
        SMALL_SPARK = 1,
        SMALL_SMOKE,
        SMALL_IMPACT
    }
}