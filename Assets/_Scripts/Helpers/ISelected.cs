namespace Helpers {

    using UnityEngine;

    public interface ISelected {
        Vector3 CurrentPoint { get; }
        Vector3 PreviousPoint { get; }
        IHasHealth CurrentTarget { get; }
        IHasHealth PreviousTarget { get; }

        bool SetPoint(Vector3 point);
        bool SetTarget(IHasHealth target);

        //bool HoverPoint(Vector3 point);
        //bool HoverTarget(IHasHealth target);
    }
}