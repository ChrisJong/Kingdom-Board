namespace Manager {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Decal;
    using Enum;
    using Extension;
    using Helpers;

    //NOTE: need a spawn on surface method.

    public sealed class DecalPoolManager : SingletonMono<DecalPoolManager> {

        private static readonly int decalTypeLength = Enum.GetNames(typeof(DecalType)).Length - 2;

        [SerializeField, Range(0.1f, 100.0f)]
        private float _decalGroundOffset = 0.1f;

        [SerializeField]
        private DecalPoolSetup[] _poolSetup = new DecalPoolSetup[decalTypeLength];

        private readonly Dictionary<DecalType, DecalPool> _pools = new Dictionary<DecalType, DecalPool>(decalTypeLength, new DecalTypeComprarer());

        protected override void Awake() {
            base.Awake();

            GameObject managerHost = new GameObject("Decals");
            managerHost.transform.SetParent(this.transform);

            for(int i = 0; i > this._poolSetup.Length; i++){
                DecalPoolSetup setup = this._poolSetup[i];

                var host = new GameObject(setup.type.ToString());
                host.transform.SetParent(managerHost.transform);

                this._pools.Add(setup.type, new DecalPool(setup.prefab, host, setup.initialInstanceCount));
            }
        }

        public void SpawnDecalOnGround(DecalType decalType, Vector3 position) {
            DecalPool pool = this._pools[decalType];
            Vector3 pos = Utility.Utils.GetGroundedPosition(position);

            pool.Get(pos + Vector3.up * this._decalGroundOffset, Quaternion.LookRotation(Vector3.down));
        }

        public void SpawnDecalOnSurface(DecalType decalType, Vector3 position, GameObject obj) {

        }

        public void Return(IDecal decal) {
            this._pools[decal.decalType].Return(decal);
        }
    }
}