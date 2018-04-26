namespace Enum {

    public enum PlayerMode {
        NONE = 0,
        RESEARCH = 1,
        UNIT_ATTACK,
        UNIT_MOVE,
        STRUCTURE_ATTACK,
        STRUCTURE_MOVE,
        STRUCTURE_SPAWN,
    };

    public enum PlayerState {
        NONE = 0,
        START = 1,
        PLAYING,
        WAITING,
        END,
        DEAD
    };
}