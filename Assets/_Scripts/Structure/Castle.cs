namespace Structure {

    using UnityEngine;

    using Enum;

    public sealed class Castle : SpawnStructureBase {
        public override StructureType structureType { get { return StructureType.CASTLE; }}

        public override bool ReceiveDamage(float damage) {
            bool isDead = base.ReceiveDamage(damage);
            //if(isDead)
                // NOTE: call player Death & End Game.
            return isDead;
        }

        public bool SpawnUnit(UnitType type) {
            return this.HandleSpawnUnit(type);
        }

    }
}