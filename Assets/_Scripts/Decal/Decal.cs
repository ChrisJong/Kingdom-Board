namespace KingdomBoard.Decal {

    using Enum;
    using Helpers;

    /// <summary>
    /// Components representing decals, which are visual effects on the ground as a result of explosions or similar effects.
    /// </summary>
    public sealed class Decal : ObjectPoolBase, IDecal {

        /// <summary>
        /// Gets or sets the type of the decal.
        /// </summary>
        /// <value>
        /// The type of the decal.
        /// </value>
        public DecalType decalType { get; set; }
    }
}