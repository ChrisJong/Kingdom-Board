namespace Manager {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;
    using Extension;
    using Helpers;
    using Player;
    using Structure;
    using Utility;

    public sealed class StructurePoolManager : SingletonMono<StructurePoolManager> {
        private static readonly int structureTypeLength = Enum.GetNames(typeof(StructureType)).Length - 2;

        [SerializeField]
        private StructurePoolSetup[] _poolSetup = new StructurePoolSetup[structureTypeLength];

        private readonly Dictionary<StructureType, STructurePool> _pools = new Dictionary<StructureType, STructurePool>(structureTypeLength, new StructureTypeComparer());

        protected override void Awake() {
            base.Awake();

            var managerHost = new GameObject("Structures");
            managerHost.transform.SetParent(this.transform);

            for(int i = 0; i < this._poolSetup.Length; i++) {
                var setup = this._poolSetup[i];

                var host = new GameObject(setup.type.ToString());
                host.transform.SetParent(this.transform);

                this._pools.Add(setup.type, new STructurePool(setup.prefab, host, setup.initialInstanceCount));
            }
        }

        /*public bool BuildStructuree(StructureType type) {
            if(type == StructureType.NONE || type == StructureType.ANY) {
                Debug.LogError(this.ToString() + " cannot spawn structure of type (not supported): " + type);
                return false;
            }
        }*/

        public Castle_v2 GetStartCastle(Player player) {
            return player.Castle ?? (Castle_v2)InternalBuild(StructureType.CASTLE, player, player.SpawnLocation.position);
        }

        private IStructure InternalBuild(StructureType type, Player player, Vector3 position) {
            var pool = this._pools[type];

            var pos = Utils.GetGroundedPosition(position);
            var structure = pool.Get(pos, Quaternion.identity);
            structure.controller = player;
            //structure.isReady = costs.time == 0.0f;

            // NOTE: change the color render to tteam color.
            // NOTE: add this to the structure list in the controller.

            return structure;
        }

        public void Return(IStructure structure) {
            this._pools[structure.structureType].Return(structure);
        }
    }

}