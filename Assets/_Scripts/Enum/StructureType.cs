namespace Enum {

    /// <summary>
    /// Enum type for the different kind of structures in the game.
    /// NONE and ANY values are used for searching generic elements.
    /// </summary>
    public enum StructureType {
        NONE = 0,
        ANY = ~0,
        CASTLE = 1
    }
}