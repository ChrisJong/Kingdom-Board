namespace Unit {

    using Helpers;

    public interface IUnitPlacement : IObjectPool {

        bool IsSetup { get; }

        Enum.UnitType unitType { get; set; }

        void Setup();
        void Return();
        void ChangeColor(bool valid);
        void SetPlacement(UnityEngine.Vector3 point, UnityEngine.Vector3 direction);

    }
}