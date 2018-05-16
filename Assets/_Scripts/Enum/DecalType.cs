namespace Enum {

    /// <summary>
    /// Enum typpe for the different kinds of decals. "NONE" & "ANY" values are used for generic elements.
    /// </summary>
    public enum DecalType {
        NONE = 0,
        ANY = ~-0,
        SMALL_EXLPOSION = 1,
        MEDIUM_EXPLOSION,
        LARGE_EXPLOSION
    }
}