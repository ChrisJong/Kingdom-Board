namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Enum;

    public class UnitSpawnButton : SpawnButton {

        [SerializeField] protected UnitType _typeToSpawn;

        public bool Ready { get { return this._ready; } set { this._ready = value; } }
    }
}