namespace KingdomBoard.Enum {

    // NOTE: need new names for differernt types of particles/

    /// <summary>
    /// Enum Type for all pooled particles (Sizes: SMALL/MEDIUM/LARGE)
    /// </summary>
    public enum ParticleType {
        NONE = 0,
        ANY = ~0,
        IMPACT_MELEE = 1,
        IMPACT_RANGE,
        IMPACT_CLERIC_ATTACK,
        IMPACT_CLERIC_HEAL,
        IMPACT_MAGE_ATTACK,
        IMPACT_WIZARD_ATTACK
    }
}