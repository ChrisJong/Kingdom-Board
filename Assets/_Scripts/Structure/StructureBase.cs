namespace Structure {

    using UnityEngine;

    using Enum;
    using Helpers;
    using System;

    [RequireComponent(typeof(UnityEngine.AI.NavMeshObstacle))]
    public abstract class StructureBase : HasHealthBase, IStructure {
        public abstract StructureType structureType { get; }
        public override EntityType entityType { get { return EntityType.STRUCTURE; } }

        public bool isReady { get; set; }

        protected override void OnEnable() {
            base.OnEnable();
            this.currentHealth = this._maxHealth;
        }

        public override bool ReceiveDamage(float damage) {
            if(this.isDead)
                return true;

            var lookUp = Quaternion.LookRotation(Vector3.up);


            this.currentHealth -= damage;
            if(this.currentHealth <= 0.0f) {
                // NOTE: remove this structure from the player list, if it isnt a castle

                // NOTE: spawn in particle effects.
                this.ReturnStructure();
                return true;
            }
            return false;
        }

        private void ReturnStructure() {
            // NOTE: return this structure into the structurepoolmanager
        }
    }
}