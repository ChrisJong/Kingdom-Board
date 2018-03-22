namespace Helpers {

    using UnityEngine;

    public interface IObjectPool {
        uint id { get; }
        GameObject gameObject { get; }
        Transform transform { get; }
    }
}