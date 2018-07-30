namespace Helpers {

    using UnityEngine;

    public interface ICanMove : IHasHealth {
        float moveSpeed { get; }
        float currentStamina { get; }
        bool isMoving { get; }
        bool isIdle { get; }
        bool canMove { get; }

        Enum.MovementType movementType { get; }

        void MoveTo(Vector3 dest);
        void StopMoving();
        void LookAt(Vector3 pos);
    }
}