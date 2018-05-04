namespace Structure {

    using System;

    using UnityEngine;

    using Enum;
    using Helpers;
    using Manager;

    [RequireComponent(typeof(UnityEngine.AI.NavMeshObstacle))]
    public abstract class StructureBase : HasHealthBase, IStructure {
        public abstract StructureType structureType { get; }
        public override EntityType entityType { get { return EntityType.STRUCTURE; } }

        public bool isReady { get; set; }

        protected override void OnEnable() {
            base.OnEnable();
            this.currentHealth = this._maxHealth;
        }

        public override bool ReceiveDamage(float damage, IHasHealth target) {
            if(this.isDead)
                return true;

            //var lookUp = Quaternion.LookRotation(Vector3.up);

            this.currentHealth -= damage;
            if(this.currentHealth <= 0.0f) {

                if(this.controller != null && this.controller.structures != null)
                    this.controller.structures.Remove(this);

                // NOTE: spawn in particle effects.
                this.ReturnStructure();
                return true;
            }
            return false;
        }

        private void ReturnStructure() {
            StructurePoolManager.instance.Return(this);
        }
    }
}