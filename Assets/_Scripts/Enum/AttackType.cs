namespace KingdomBoard.Enum {

    /// <summary>
    /// Enum type for the different kind of damage attrubytes for units in the game.
    /// NONE and ANY values are used for searching generic elements.
    /// </summary>
    public enum AttackType {
        NONE = 0,
        ANY = ~0,
        PHYSICAL = 1,
        MAGIC,
        PROJECTFILE
    }
}