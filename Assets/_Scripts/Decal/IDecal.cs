namespace KingdomBoard.Decal {

    using Enum;
    using Helpers;

    /// <summary>
    /// Interface representing decals.
    /// </summary>
    public interface IDecal : IObjectPool {
        /// <summary>
        /// Gets or set the type of decal.
        /// </summary>
        /// <value>
        /// the type of decal.
        /// </value>
        DecalType decalType { get; set; }
    }
}