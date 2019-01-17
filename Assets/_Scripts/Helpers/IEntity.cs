namespace Helpers {
    
    using UnityEngine;

    using Enum;

    public interface IEntity : IObjectPool {
        EntityType entityType { get; }
        Vector3 position { get; set; }
        Quaternion rotation { get; set; }
        bool isActive { get; set; }
        bool IsSetup { get; set; }

        void Setup();
        void Init();
        void Return();
    }
}