namespace Structure {

    using UnityEngine;

    using Enum;
    using Manager;

    [System.Serializable]
    public abstract class SpawnStructureBase : StructureBase {
        #region VARIABLE

        protected float _spawnRange = 5.0f;

        protected int _queueLimit = 0;

        protected uint _unitQueueCount = 0; // used to for unit queue id (the number shouldn't decrease)
        protected uint _lastQueueID = 0;

        public float SpawnRange { get { return this._spawnRange; } }

        public int QueueLImit { get { return this._queueLimit; } }

        public uint UnitQueueCount { get { return this._unitQueueCount; } }
        public uint LastQueueID { get { return this._lastQueueID; } }

        #endregion

        #region CLASS
        public override void Setup() {
            base.Setup();

            this._queueLimit = this._data.queueLimit;
            this._spawnRange = this._data.spawnRange;
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