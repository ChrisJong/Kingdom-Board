namespace Enum {

    /// <summary>
    /// Enum type for the different kind of units in the game.
    /// NONE and ANY values are used for searching generic elements.
    /// </summary>
    public enum UnitType {
        NONE = 0,
        ANY = ~0,
        ARCHER = 1, // Done
        CROSSBOW, // Done
        LONGBOW, // Done
        MAGE, // Done
        CLERIC, // Incomplete (Done, but doesn't have the healing spell)
        WIZARD, // Doing
        WARRIOR, // Done
        KNIGHT
    }
}