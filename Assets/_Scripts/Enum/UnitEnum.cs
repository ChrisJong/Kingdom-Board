 namespace Enum {

    public enum UnitState {
        NONE = 0,
        ANY = ~0,
        SPAWN = 1,
        IDLE,
        ATTACK_STANDBY,
        ATTACK_ANIMATION,
        ATTACK,
        HEAL_STANDBY,
        HEAL_ANIMATION,
        HEAL,
        SPECIAL_STANDBY,
        SPECIAL_ANIMATION,
        SPECIAL, // Anything to do with casting spells or a secondary attack.
        MOVING_STANDBY,
        MOVING,
        FINISH,
        DEAD
    }

    public enum UnitClassType {
        NONE = 0,
        ANY = ~0,
        MELEE = 1,
        RANGE = 2,
        MAGIC = 3
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


    public enum UnitUpgradeType {
        NONE = 0,
        ANY = ~0,
        HEALTH = 1,
        ATTACK,
        STAMINA
    }

    public enum UnitUpgrade { // To Remove
        NONE = 0,
        ANY = ~0,
        HEALTH = 1,
        ATTACK = 2,
        STAMINA = 3,
        ATTACK_AOE,
        SPLASH_AOE,
        HEALING_ATTACK,
        HEALING_AOE
    }
}