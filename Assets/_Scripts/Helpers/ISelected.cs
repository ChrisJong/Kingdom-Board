namespace Helpers {

    using UnityEngine;

    public interface ISelected {
        Vector3 currentPoint { get; }
        Vector3 previousPoint { get; }
        IHasHealth currentTarget { get; }
        IHasHealth previousTarget { get; }

        bool SetPoint(Vector3 point);
        bool SetTarget(IHasHealth target);
    }
}