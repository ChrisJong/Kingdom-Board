namespace Enum {

    public enum UnitState {
        NONE = 0,
        ANY = ~0,
        IDLE = 1,
        ATTACKING,
        SPECIAL, // Anything to do with casting spells or a secondary attack.
        MOVING,
        FINISHED,
    }

    /// <summary>
    /// Enum type for the different kind of units in the game.
    /// NONE and ANY values are used for searching generic elements.
    /// </summary>
    public enum UnitType {
        NONE = 0,
        ANY = ~0,
        ARCHER = 1,
        CROSSBOW,
        LONGBOW,
        MAGE,
        CLERIC,
        WIZARD,
        WARRIOR,
        KNIGHT,
        GUARDIAN
    }
}