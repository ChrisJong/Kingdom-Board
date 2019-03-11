namespace KingdomBoard.Enum {

    /// <summary>
    /// Enum type for the different kind of entities in the game.
    /// NONE and ANY values are used for searching generic elements.
    /// </summary>
    public enum EntityType {
        NONE = 0,
        ANY = ~0,
        STRUCTURE = 1,
        UNIT,
    }
}