namespace Structure {

    using UnityEngine;

    using Enum;
    using Manager;

    public abstract class SpawnStructureBase : StructureBase {
        [Header("SPAWN"), SerializeField]
        protected float _spawnDistance = 6.0f;
        public float spawnDistance { get { return this._spawnDistance; } }
        protected float _anglePerSpawn = 15.0f;
        protected float _lastSpawn;
        protected int _lastSpawnIndex = 0;

        // NOTE: spawnQueue list. 
        // NOTE: need a class called. SpawnQueue. which inherits from StructureQueue & UnitQueue (contains the type of entity to queue and a turn timer countdown.)

        public bool inCooldown { get { return Time.timeSinceLevelLoad < this._lastSpawn; } }

        protected bool HandleSpawnUnit(UnitType type) {
            if(type == UnitType.NONE || type == UnitType.ANY) {
                Debug.LogError(this.ToString() + " cannot spawn unit of type (not supported): " + type);
                return false;
            }

            return UnitPoolManager.instance.SpawnUnit(type, this.controller, this.position, this._spawnDistance, this._anglePerSpawn, ref this._lastSpawnIndex);
        }
    }
}