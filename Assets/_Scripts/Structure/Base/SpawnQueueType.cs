namespace Structure {

    using System;

    using UnityEngine;

    using Enum;
    using UI;

    [Serializable]
    public sealed class SpawnQueueType {
        public uint id = 0; // the id should match the id in the UnitQueueButton script
        public int counter = 0;

        public bool ready = false;

        public UnitType type;

        public SpawnQueueType(uint id, UnitType type) {
            this.type = type;
            this.id = id;
            this.counter = 1;
        }

        public SpawnQueueType(uint id, UnitType type, int count) {
            this.type = type;
            this.id = id;
            this.counter = count;
        }

        public bool Countdown() {
            --this.counter;

            if(this.counter <= 0) {
                this.ready = true;
                return true;
            }

            return false;
        }
    }
}