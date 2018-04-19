namespace Structure {

    using System;

    using UnityEngine;

    using Enum;

    [Serializable]
    public sealed class SpawnQueueType {
        public UnitType type;
        public int counter;
        public bool ready = false;

        public SpawnQueueType(UnitType type, int count) {
            this.type = type;
            this.counter = count;
        }

        public void Countdown() {
            --this.counter;

            if(this.counter <= 0)
                this.ready = true;
        }
    }
}