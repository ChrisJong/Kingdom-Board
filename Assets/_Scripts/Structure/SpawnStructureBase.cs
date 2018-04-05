namespace Structure {

    using UnityEngine;

    using Enum;
    using Manager;

    public abstract class SpawnStructureBase : StructureBase {
        protected float _spawnDistance = 6.0f;
        protected float _anglePerSpawn = 15.0f;
        protected float _lastSpawn;
        protected int _lastSpawnIndex;

        // NOTE: spawnQueue list. 
        // NOTE: need a class called. SpawnQueue. which inherits from StructureQueue & UnitQueue (contains the type of entity to queue and a turn timer countdown.)

        public bool inCooldown { get { return Time.timeSinceLevelLoad < this._lastSpawn; } }

        protected void CheckSpawnQueue() {
            // NOTE: Decrease spawn queue timer.
            // NOTE: check if any spawns hit 0.
            // NOTE: call HandleSpawnUnit to spawn any units that have a 0 queue timer.
            // NOTE: dequeue the spawnqueue from the list.
            // NOTE: sort the queue from lowerest to highest in terms of timer.
        }

        protected bool AddUnitToQueue(UnitType type) {
            // NOTE: check player unit cap.
            // NOTE: add new unit to queue if unit cap isnt maxed.
            return false;
        }

        protected bool HandleSpawnUnit(UnitType type) {
            if(type == UnitType.NONE || type == UnitType.ANY) {
                Debug.LogError(this.ToString() + " cannot spawn unit of type (not supported): " + type);
                return false;
            }

            // NOTE: Add unit to spawn queue.

            // NOTE: charge resouce amount for unit onto the player.

            return UnitPoolManager.instance.SpawnUnit(type, this.controller, this.position, this._spawnDistance, this._anglePerSpawn, this._lastSpawnIndex);
        }
    }
}