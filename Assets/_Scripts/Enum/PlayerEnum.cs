namespace Enum {

    public enum SelectionState {
        NONE = 0,
        ANY = ~0,
        FREE = 1,
        STANDBY, // Entity Selected.
        SPAWNPOINT,
        WAITING,
        END
    };

    public enum PlayerState {
        NONE = 0,
        START = 1,
        ATTACKING, // Playing State.
        DEFENDING, // Playing State.
        WAITING,
        END,
        DEAD
    };

    public enum ResearchState {
        NONE = 0,
        ANY = ~0,
        START = 1,
        CLASS,
        UNIT,
        UPGRADE,
        BACK,
        FINISHED
    };

    public enum CursorState {
        NONE = 0,
        ANY = ~0,
        DEFAULT = 1,
        SELECTED,
        MOVE,
        MOVE_NOTREADY,
        ATTACK,
        ATTACK_NOTREADY,
        HEAL,
        HEAL_NOTREADY,
    };

    public enum PlayerResource {
        NONE = 0,
        ANY = ~0,
        GOLD = 1,
        POPULATION
    }
}