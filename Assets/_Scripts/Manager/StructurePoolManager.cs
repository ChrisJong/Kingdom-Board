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
    using UI;
    using Utility;

    public sealed class StructurePoolManager : SingletonMono<StructurePoolManager> {

        private static readonly int structureTypeLength = Enum.GetNames(typeof(StructureType)).Length - 2;

        [SerializeField]
        private StructurePoolSetup[] _poolSetup = new StructurePoolSetup[structureTypeLength];
        private readonly Dictionary<StructureType, StructurePool> _pools = new Dictionary<StructureType, StructurePool>(structureTypeLength, new StructureTypeComparer());

        protected override void Awake() {
            base.Awake();

            var managerHost = new GameObject("Structures");
            managerHost.transform.SetParent(this.transform);

            for(int i = 0; i < this._poolSetup.Length; i++) {
                var setup = this._poolSetup[i];

                var host = new GameObject(setup.type.ToString());
                host.transform.SetParent(this.transform);

                this._pools.Add(setup.type, new StructurePool(setup.prefab, host, setup.initialInstanceCount));
            }
        }

        /*public bool BuildStructuree(StructureType type) {
            if(type == StructureType.NONE || type == StructureType.ANY) {
                Debug.LogError(this.ToString() + " cannot spawn structure of type (not supported): " + type);
                return false;
            }
        }*/

        public Castle GetStartCastle(Player player) {
            return player.castle ?? (Castle)InternalBuild(StructureType.CASTLE, player, player.spawnLocation.position);
        }

        private IStructure InternalBuild(StructureType type, Player controller, Vector3 position) {
            var pool = this._pools[type];

            var pos = Utils.GetGroundedPosition(position);
            var structure = pool.Get(pos, controller.spawnLocation.rotation);
            structure.controller = controller;

            structure.uiComponent = structure.transform.GetComponentInChildren<CastleUI>() as ScreenSpaceUI;
            structure.uiComponent.controller = controller;
            structure.gameObject.ColorRenderers(controller.color);
            ((CastleUI)structure.uiComponent).Hide();
            //structure.isReady = costs.time == 0.0f;

            // NOTE: change the color render to tteam color.
            controller.structures.Add(structure);

            return structure;
        }

        public void Return(IStructure structure) {
            this._pools[structure.structureType].Return(structure);
        }
    }

}