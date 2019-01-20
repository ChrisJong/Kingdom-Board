namespace Structure {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;
    using Helpers;
    using Manager;
    using Scriptable;
    using UI;
    using Utility;
    using Player;

    [System.Serializable, RequireComponent(typeof(CastleUI))]
    public sealed class Castle : SpawnStructureBase {

        #region VARIABLE
        public LineRenderDrawRectangle radiusDrawer = null;

        private List<SpawnQueueType> _spawnQueue;
        private SpawnQueueType _toSpawn = null;

        public CastleUI castleUI { get { return this.uiBase as CastleUI; } }
        public List<SpawnQueueType> SpawnQueue { get { return this._spawnQueue; } }
        public SpawnQueueType ToSpawn { get { return this._toSpawn; } set { this._toSpawn = value; } }
        #endregion

        #region CLASS
        public override void Setup() {
            base.Setup();

            this.castleUI.Setup(this);
        }

        public override void Init(Player contoller) {

            this._maxHealth = this._data.health;
            this._queueLimit = this._data.queueLimit;
            this._spawnRange = this._data.spawnRange;

            base.Init(contoller);

            this.castleUI.Init(this.controller);

            this._spawnQueue = new List<SpawnQueueType>(this._queueLimit);
            this._unitQueueCount = 0;

            this.radiusDrawer.Draw(this._colliderBounds, this._spawnRange);
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

        public bool AddUnitToQueue(UnitClassType classType, UnitType unitType) {

            UnitScriptable unitData = UnitPoolManager.instance.FetchUnitData(classType, unitType);

            if(this._spawnQueue.Count >= this.QueueLImit)
                return false;

            if(ResourceManager.instance.SpendResource(this.controller, PlayerResource.GOLD, unitData.goldCost) &&
               ResourceManager.instance.SpendResource(this.controller, PlayerResource.POPULATION, unitData.populationCost)) {

                // Queue Unit.
                SpawnQueueType temp = new SpawnQueueType(this._unitQueueCount, unitType, unitData.turnCost, unitData.goldCost, unitData.populationCost);
                this.castleUI.AddToQueue(unitData, temp);

                this._unitQueueCount++;
                this._spawnQueue.Add(temp);

                return true;
            }

            return false;
        }

        public bool RemoveUnitFromQueue(SpawnQueueType queue) {
            if(!this._spawnQueue.Contains(queue))
                return false;

            // NOte: return resources back to the player since the unit wasn't ready to spawn (probably a change in preference)
            //this.controller.AddResource(this.GetUnitResourceCost(toRemove.type));
            //this.controller.RemoveFromUnitCap(this.GetUnitCapCost(toRemove.type));

            this.castleUI.RemoveFromQueue(queue);
            this._spawnQueue.Remove(queue);

            return true;
        }

        public bool SpawnUnit(Vector3 position) {
            // Check to see if the position is correct.
            float distance = Vector3.Distance(Utils.ClosesPointToBounds(this._colliderBounds, position), position);
            Debug.Log("Distance From CAstle: " + distance.ToString());

            if(distance > this._spawnRange) {
                return false;
            }

            if(this.HandleSpawnUnit(this._toSpawn.type, position)) {

                this.castleUI.RemoveFromQueue(this._toSpawn);
                this.SpawnQueue.Remove(this._toSpawn);

                this._toSpawn.FinishedSpawn();

                this.controller.playerSelect.CurrentState = SelectionState.FREE;
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

            this.controller.playerSelect.CurrentState = SelectionState.SELECT_SPAWNPOINT;
            this.structureState = StructureState.SPAWN;
            this.radiusDrawer.SetActive(true);
            this._toSpawn = queue;

            return true;
        }

        public bool CancelSpawn() {
            this._toSpawn.CancelSpawn();
            this._toSpawn = null;

            this.radiusDrawer.SetActive(false);
            this._structureState = StructureState.IDLE;

            return true;
        }

        public void UnlockUnitToSpawn(UnitClassType classType, UnitType unitType) {
            this.castleUI.UnlockSpawnButton(classType, unitType);
        }
        #endregion
    }
}