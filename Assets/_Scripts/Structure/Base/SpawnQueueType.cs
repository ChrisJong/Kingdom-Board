namespace Structure {

    using System;

    using UnityEngine;

    using Enum;
    using UI;

    [Serializable]
    public sealed class SpawnQueueType {
        public uint id = 0; // the id should match the id in the UnitQueueButton script
        public int counter = 0;

        public float goldCost = 0;
        public float populationCost = 0;

        public bool ready = false;

        public UnitType type = UnitType.NONE;
        public QueueButton queueButton = null;

        public SpawnQueueType(uint id, UnitType type) {
            this.type = type;
            this.id = id;
            this.counter = 1;
        }

        public SpawnQueueType(uint id, UnitType type, int count, float goldCost, float populationCost) {
            this.type = type;
            this.id = id;
            this.counter = count;
            this.goldCost = goldCost;
            this.populationCost = populationCost;
        }

        public void SetQueueButton(QueueButton button) {
            this.queueButton = button;
        }

        public bool Countdown() {
            --this.counter;

            if(this.counter <= 0) {
                this.ready = true;
                this.queueButton.Ready();
                return true;
            }

            return false;
        }

        public void CancelSpawn() {
            this.queueButton.CancelSpawn();
        }


        public void FinishedSpawn() {
            this.queueButton.FinishSpawn();
        }

        public void Remove() {
            this.queueButton = null;
        }
    }
}