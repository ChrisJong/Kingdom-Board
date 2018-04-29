namespace Structure {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using UI;

    [RequireComponent(typeof(CastleUI))]
    public sealed class Castle : SpawnStructureBase {

        [SerializeField]
        private List<SpawnQueueType> _spawnQueue = new List<SpawnQueueType>();

        public override StructureType structureType { get { return StructureType.CASTLE; } }

        public List<SpawnQueueType> spawnQueue { get { return this._spawnQueue; } }

        public override bool ReceiveDamage(float damage, IHasHealth target) {
            bool isDead = base.ReceiveDamage(damage, target);
            if(isDead)
                this.controller.OnDeath();
            return isDead;
        }

        public void CheckSpawnQueue() {
            if(this._spawnQueue.Count <= 0 || this._spawnQueue == null)
                return;

            // NOTE: dequeue the spawnqueue from the list.
            // NOTE: sort the queue from lowerest to highest in terms of timer.
            List<SpawnQueueType> spawns = new List<SpawnQueueType>();

            // NOTE: need to find a cleaner way to remove units that are marked spawn from the list.
            // NOTE: throws an InvalidOperationException since i can't edit the last while its being used/referecned in the forloop.
            foreach(SpawnQueueType spawn in this._spawnQueue) {
                spawn.Countdown();

                if(spawn.ready) {
                    UnitType type = spawn.type;
                    if(this.SpawnUnit(type)) {
                        spawns.Add(spawn);
                    }
                }
            }

            foreach(SpawnQueueType removeal in spawns) {
                if(this._spawnQueue.Contains(removeal))
                    this._spawnQueue.Remove(removeal);
                else
                    Debug.LogError("COULDN'T FIND THE UNIT " + removeal.type.ToString() + " TO REMOVE");
            }
        }

        public bool AddUnitToQueue(UnitType type) {
            // NOTE: check player unit cap.
            // NOTE: check player resource.
            // NOTE: add new unit to queue if unit cap isnt maxed & there are resources avaliable.
            int unitCapCost = this.GetUnitCapCost(type);
            int unitSpawnCounter = this.GetUnitSpawnCounter(type);
            int unitCost = this.GetUnitResourceCost(type);

            if(this.controller.HasResource(unitCost) && this.controller.CheckUnitCap(unitCapCost)) {
                this.controller.SpendResource(unitCost);
                this.controller.AddToUnitCap(unitCapCost);
                this._spawnQueue.Add(new SpawnQueueType(type, unitSpawnCounter));
                return true;
            }
            return false;
        }

        public bool SpawnUnit(UnitType type) {
            return this.HandleSpawnUnit(type);
        }

        private int GetUnitCapCost(UnitType type) {
            int cost;

            switch(type) {
                case UnitType.ARCHER:
                cost = UnitValues.ArcherValues.UNITCAPCOST;
                break;

                case UnitType.MAGE:
                cost = UnitValues.MageValues.UNITCAPCOST;
                break;

                case UnitType.WARRIOR:
                cost = UnitValues.WarriorValues.UNITCAPCOST;
                break;

                default:
                Debug.LogError("Unit of type(" + type.ToString() + ") not found");
                cost = -1;
                break;
            }

            return cost;
        }

        private int GetUnitSpawnCounter(UnitType type) {
            int cost;

            switch(type) {
                case UnitType.ARCHER:
                cost = UnitValues.ArcherValues.SPAWNCOUNT;
                break;

                case UnitType.MAGE:
                cost = UnitValues.MageValues.SPAWNCOUNT;
                break;

                case UnitType.WARRIOR:
                cost = UnitValues.WarriorValues.SPAWNCOUNT;
                break;

                default:
                Debug.LogError("Unit of type(" + type.ToString() + ") not found");
                cost = -1;
                break;
            }

            return cost;
        }

        private int GetUnitResourceCost(UnitType type) {
            int cost;

            switch(type) {
                case UnitType.ARCHER:
                cost = UnitValues.ArcherValues.SPAWNCOST;
                break;

                case UnitType.MAGE:
                cost = UnitValues.MageValues.SPAWNCOST;
                break;

                case UnitType.WARRIOR:
                cost = UnitValues.WarriorValues.SPAWNCOST;
                break;

                default:
                Debug.LogError("Unit of type(" + type.ToString() + ") not found");
                cost = -1;
                break;
            }

            return cost;
        }
    }
}