namespace KingdomBoard.Enum {

    public enum StructureState {
        NONE = 0,
        ANY = ~0,
        IDLE = 1,
        SPAWN,
        STANDBY_TARGET,
        STANDBY_POINT,
        FINISHED,
        DEAD,
    }

    public enum StructureClassType {
        NONE = 0,
        ANY = ~0,
        MAIN = 1,
        NEUTRAL,
    }

    /// <summary>
    /// Enum type for the different kind of structures in the game.
    /// NONE and ANY values are used for searching generic elements.
    /// </summary>
    public enum StructureType {
        NONE = 0,
        ANY = ~0,
        CASTLE = 1,
        GOLDMINE,
    }
}