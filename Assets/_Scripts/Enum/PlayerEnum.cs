namespace Enum {

    public enum SelectionState {
        NONE = 0,
        FREE = 1,
        STANDBY,
        SELECT_TARGET, // ANY
        SELECT_ALLYTARGET,
        SELECT_ENEMYTARGET,
        SELECT_POINT,
        SELECT_SPAWNPOINT
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