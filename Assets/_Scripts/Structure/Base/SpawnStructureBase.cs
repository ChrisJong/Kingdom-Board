namespace Structure {

    using UnityEngine;

    using Enum;
    using Manager;

    public abstract class SpawnStructureBase : StructureBase {
        #region vARIBALE
        [Header("SPAWN STRUCTURE")]
        protected float _spawnDistance = 5.0f;
        protected float _anglePerSpawn = 15.0f;
        protected float _lastSpawn;

        [SerializeField] protected int _queueLimit = 10;
        protected uint _lastQueueID = 0;
        protected uint _unitQueueCount = 0; // used to for unit queue id (the number shouldn't decrease)

        // NOTE: spawnQueue list. 
        // NOTE: need a class called. SpawnQueue. which inherits from StructureQueue & UnitQueue (contains the type of entity to queue and a turn timer countdown.)

        public float spawnDistance { get { return this._spawnDistance; } }

        public int queueLImit { get { return this._queueLimit; } }
        public uint lastQueueID { get { return this._lastQueueID; } }
        public uint unitQueueCount { get { return this._unitQueueCount; } }

        public bool inCooldown { get { return Time.timeSinceLevelLoad < this._lastSpawn; } }
        #endregion

        #region CLASS
        protected bool HandleSpawnUnit(UnitType type) {
            if(type == UnitType.NONE || type == UnitType.ANY) {
                Debug.LogError(this.ToString() + " cannot spawn unit of type (not supported): " + type);
                return false;
            }

            this._lastQueueID = this._unitQueueCount;
            return UnitPoolManager.instance.SpawnUnit(type, this.Controller, this.position, this._spawnDistance, this._anglePerSpawn, ref this._lastQueueID);
        }

        protected bool HandleSpawnUnit(UnitType type, Vector3 position) {
            if(type == UnitType.NONE || type == UnitType.ANY) {
                Debug.LogError(this.ToString() + " cannot spawn unit of type (not Supported): " + type);
                return false;
            }

            this._lastQueueID = this._unitQueueCount;

            return UnitPoolManager.instance.SpawnUnit(type, this.Controller, position);
        }
        #endregion
    }
}