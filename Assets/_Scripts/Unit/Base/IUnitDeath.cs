namespace KingdomBoard.Unit {

    using Helpers;

    public interface IUnitDeath : IObjectPool {

        bool IsSetup { get; }

        int TurnCounter { get; }

        Enum.UnitType unitType { get; set; }

        void Setup();
        void Return();
        void Init(UnityEngine.Color color, UnityEngine.Vector3 eDirection, UnityEngine.Vector3 ePoseition, float eForce, int counter);

        void Countdown();
    }
}