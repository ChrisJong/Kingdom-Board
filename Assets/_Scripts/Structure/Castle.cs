namespace Structure {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using Scriptable;
    using UI;
    using Utility;

    [RequireComponent(typeof(CastleUI))]
    public sealed class Castle : SpawnStructureBase {

        #region VARIABLE

        private CastleUI _ui = null;
        public LineRenderDrawRectangle radiusDrawer = null;

        [Header("CASTLE - SPAWN")]
        [SerializeField] private List<SpawnQueueType> _spawnQueue;
        private SpawnQueueType _toSpawn = null;

        public CastleUI UI { get { return this._ui; } }

        public List<SpawnQueueType> spawnQueue { get { return this._spawnQueue; } }
        public override StructureType structureType { get { return StructureType.CASTLE; } }

        public SpawnQueueType toSpawn { get { return this._toSpawn; } set { this._toSpawn = value; } }
        #endregion

        #region CLASS
        public override void Init() {
            base.Init();

            this._spawnQueue = new List<SpawnQueueType>(this._queueLimit);
            this._unitQueueCount = 0;

            this._ui = this.transform.GetComponent<CastleUI>() as CastleUI;
            this._ui.Init(this);

            this.radiusDrawer.Draw(this._colliderBounds, this._spawnDistance);
            this.radiusDrawer.SetActive(false);
        }

        public override bool SetPoint(Vector3 point) {
            Vector3 position = Vector3.zero;

            if(!Utility.Utils.SamplePosition(point, out position))
                return false;

            if(this._currentPoint.HasValue) {
                this._previousPoint = this._currentPoint;
                this.debugPreviousPoint = this._previousPoint.Value;
            }

            this._currentPoint = position;
            this.debugCurrentPoint = position;

            if(this._previousPoint.HasValue && this._currentPoint.HasValue) {
                if(this._currentPoint.Value.Equals(this._previousPoint.Value))
                    return false;
            }

            if(this._structureState == StructureState.SPAWN) {
                if(!this.SpawnUnit(this._currentPoint.Value))
                    return false;
            }

            return true;
        }

        public override bool ReceiveDamage(float damage, IHasHealth target) {
            bool isDead = base.ReceiveDamage(damage, target);
            if(isDead)
                this.controller.OnDeath();
            return isDead;
        }

        public override bool ReceiveDamage(float damage, IHasHealth target, Vector3 origin) {
            return this.ReceiveDamage(damage, target);
        }

        public void CheckSpawnQueue() {
            if(this._spawnQueue.Count <= 0 || this._spawnQueue == null)
                return;

            for(int i = 0; i < this._spawnQueue.Count; i++) {
                Debug.Log("Queue Count: " + this._spawnQueue.Count + " - Counter: " + i.ToString());
                this._spawnQueue[i].Countdown();
            }
        }

        public bool AddUnitToQueue(ClassType classType, UnitType unitType) {

            /*int capCost = this.GetUnitCapCost(type);
            int counter = this.GetUnitSpawnCounter(type);
            int cost = this.GetUnitResourceCost(type);

            // Check to see if the queue limit has been reached
            if(this._spawnQueue.Count >= this.queueLImit)
                return false;

            if(this.controller.HasResource(cost) && this.controller.CheckUnitCap(capCost)) {
                this.controller.SpendResource(cost);
                this.controller.AddToUnitCap(capCost);

                SpawnQueueType temp = new SpawnQueueType(this._unitQueueCount, type, counter);
                this._spawnQueue.Add(temp);
                this._unitQueueCount++;
                return true;
            }
            return false;*/

            UnitScriptable unitData = UnitPoolManager.instance.FetchUnitData(classType, unitType);

            if(this._spawnQueue.Count >= this.queueLImit)
                return false;

            // NOTE: Resource and Cap limit checks go here.

            SpawnQueueType temp = new SpawnQueueType(this._unitQueueCount, unitType, unitData.turnCost, unitData.goldCost, unitData.populationCost);
            this._ui.AddToQueue(unitData, temp);

            this._unitQueueCount++;
            this._spawnQueue.Add(temp);

            return true;
        }

        public bool RemoveUnitFromQueue(SpawnQueueType queue) {
            if(!this._spawnQueue.Contains(queue)) {
                Debug.LogError("Spawn Queue Doesn't Contain Unit of Type: " + queue.type.ToString());
                return false;
            }

            // NOte: return resources back to the player since the unit wasn't ready to spawn (probably a change in preference)
            //this.controller.AddResource(this.GetUnitResourceCost(toRemove.type));
            //this.controller.RemoveFromUnitCap(this.GetUnitCapCost(toRemove.type));

            this._ui.RemoveFromQueue(queue);
            this._spawnQueue.Remove(queue);

            return true;
        }

        public bool SpawnUnit(UnitType type) {
            return this.HandleSpawnUnit(type);
        }

        public bool SpawnUnit(UnitType type, Vector3 position) {
            return this.HandleSpawnUnit(type, position);
        }

        public bool SpawnUnit(SpawnQueueType queue) {
            return this.HandleSpawnUnit(queue.type);
        }

        public bool SpawnUnit(Vector3 position) {
            // Check to see if the position is correct.
            float distance = Vector3.Distance(Utils.ClosesPointToBounds(this._colliderBounds, position), position);
            Debug.Log("Distance From CAstle: " + distance.ToString());

            if(distance > this._spawnDistance) {
                return false;
            }

            if(this.HandleSpawnUnit(this._toSpawn.type, position)) {

                this._ui.RemoveFromQueue(this._toSpawn);
                this.spawnQueue.Remove(this._toSpawn);

                this._toSpawn.FinishedSpawn();

                this.controller.selectionState = SelectionState.FREE;
                this.structureState = StructureState.IDLE;
                this.radiusDrawer.SetActive(false);

                this._toSpawn = null;

                return true;

            } else
                return false;
        }

        public bool SetSpawn(SpawnQueueType queue) {

            if(!this._spawnQueue.Contains(queue)) {
                Debug.LogError("Spawn Queue Doesn't Contain Unit of Type: " + queue.type.ToString());
                return false;
            }

            if(this._toSpawn != null)
                this._toSpawn.CancelSpawn();

            this.controller.selectionState = SelectionState.SELECT_SPAWNPOINT;
            this.structureState = StructureState.SPAWN;
            this.radiusDrawer.SetActive(true);
            this._toSpawn = queue;

            return true;
        }

        // NOTE: REMOVE
        public bool SetSpawn(uint id) {
            this.controller.selectionState = SelectionState.SELECT_POINT;
            this.structureState = StructureState.SPAWN;
            this.radiusDrawer.SetActive(true);

            // Grab the unit to spawn from the queue list.
            this._toSpawn = null;
            foreach(SpawnQueueType queue in this._spawnQueue) {
                if(queue.id == id) {
                    this._toSpawn = queue;
                    break;
                }
            }

            if(this._toSpawn == null)
                return false;

            return true;
        }

        public bool CancelSpawn() {
            this._toSpawn.CancelSpawn();
            this._toSpawn = null;

            this.radiusDrawer.SetActive(false);
            this._structureState = StructureState.IDLE;

            return true;
        }

        public void UnlockUnitToSpawn(ClassType classType, UnitType unitType) {
            this._ui.UnlockSpawnButton(classType, unitType);
        }
        #endregion
    }
}