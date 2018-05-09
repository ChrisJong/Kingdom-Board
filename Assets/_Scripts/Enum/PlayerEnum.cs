namespace Enum {

    public enum SelectionState {
        NONE = 0,
        FREE = 1,
        RESEARCH,
        UNIT_ATTACK,
        UNIT_SPECIAL,
        UNIT_MOVE,
        STRUCTURE_ATTACK,
        STRUCTURE_MOVE
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
}