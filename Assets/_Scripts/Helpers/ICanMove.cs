namespace Helpers {

    using UnityEngine;

    public interface ICanMove : IHasHealth {
        float MoveSpeed { get; }
        bool IsMoving { get; }
        bool IsIdle { get; }
        bool CanMove { get; }

        Enum.MovementType moveType { get; }

        void MoveTo(Vector3 dest);
        void StopMoving();
        void LookAt(Vector3 pos);
    }
}