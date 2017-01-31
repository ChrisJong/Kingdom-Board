namespace Entity {
    using UnityEngine;

    public interface IEntity {

        uint id { get; }
        int mpID { get; }
        int playerID { get; }

        GameObject gameObject { get; }

        Transform transform { get; }

        Vector3 position { get; set; }

        Quaternion rotation { get; set; }

        bool active { get; set; }
    }
}