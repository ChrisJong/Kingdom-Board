namespace Structure {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Constants;
    using Enum;

    public sealed class Castle : SpawnStructureBase {

        private List<SpawnQueueType> _spawnQueue = new List<SpawnQueueType>();

        public override StructureType structureType { get { return StructureType.CASTLE; } }

        public override bool ReceiveDamage(float damage) {
            bool isDead = base.ReceiveDamage(damage);
            //if(isDead)
                // NOTE: call player Death & End Game.
            return isDead;
        }

        public void CheckSpawnQueue() {
            // NOTE: Decrease spawn queue timer.
            // NOTE: check if any spawns hit 0.
            // NOTE: call HandleSpawnUnit to spawn any units that have a 0 queue timer.
            // NOTE: dequeue the spawnqueue from the list.
            // NOTE: sort the queue from lowerest to highest in terms of timer.

            foreach(SpawnQueueType unit in this._spawnQueue) {
                unit.Countdown();

                if(unit.ready) {
                    this.SpawnUnit(unit.type);
                    this._spawnQueue.Remove(unit);
                } 
            }
        }

        public bool AddUnitToQueue(UnitType type) {
            // NOTE: check player unit cap.
            // NOTE: add new unit to queue if unit cap isnt maxed.
            int count = this.GetUnitCount(type);

            this._spawnQueue.Add(new SpawnQueueType(type, count));

            return false;
        }

        public bool SpawnUnit(UnitType type) {
            return this.HandleSpawnUnit(type);
        }

        private int GetUnitCount(UnitType type) {
            int count;

            switch(type) {
                case UnitType.ARCHER:
                count = UnitValues.ArcherValues.SPAWNCOUNT;
                break;

                case UnitType.MAGE:
                count = UnitValues.MageValues.SPAWNCOUNT;
                break;

                case UnitType.WARRIOR:
                count = UnitValues.WarriorValues.SPAWNCOUNT;
                break;

                default:
                Debug.LogError("Unit of type(" + type.ToString() + ") not found, defaulting to a value of 5");
                count = 5;
                break;
            }

            return count;
        }
    }
}