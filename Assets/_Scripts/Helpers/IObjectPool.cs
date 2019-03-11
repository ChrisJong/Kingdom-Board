namespace KingdomBoard.Helpers {

    using UnityEngine;

    public interface IObjectPool {
        uint PoolID { get; }
        GameObject gameObject { get; }
        Transform transform { get; }
    }
}