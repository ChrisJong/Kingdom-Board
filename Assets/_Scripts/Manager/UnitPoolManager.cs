﻿namespace Manager {

    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;
    using Extension;
    using Helpers;
    using Player;
    using UI;
    using Unit;
    using Utility;

    public sealed class UnitPoolManager : SingletonMono<UnitPoolManager> {

        private static readonly int unitTypeLength = Enum.GetNames(typeof(UnitType)).Length - 2;

        [SerializeField] private UnitPoolSetup[] _poolSetup = new UnitPoolSetup[unitTypeLength];
        private readonly Dictionary<UnitType, UnitPool> _pools = new Dictionary<UnitType, UnitPool>(unitTypeLength, new UnitTypeComparer());

        [SerializeField] private List<UnitDeath> _poolDeath = new List<UnitDeath>();

        protected override void Awake() {
            base.Awake();

            var managerHost = new GameObject("Units");
            managerHost.transform.SetParent(this.transform);

            for(int i = 0; i < this._poolSetup.Length; i++) {
                var setup = this._poolSetup[i];

                var host = new GameObject(setup.type.ToString());
                host.transform.SetParent(managerHost.transform);

                this._pools.Add(setup.type, new UnitPool(setup.prefab, host, setup.initialInstanceCount));

                setup.prefab.GetComponent<UnitBase>().SetupAnimation();
            }
        }


        public bool SpawnUnit(UnitType type, Player controller, Vector3 position, float spawnDistance, float anglePerSpawm, ref int spawnIndex) {
            if(type == UnitType.NONE || type == UnitType.ANY || !this._pools.ContainsKey(type)) {
                Debug.LogError(this.ToString() + " cannot spawn unit of type (not supported): " + type);
                return false;
            }
            this.InternalSpawnUnit(type, controller, position, spawnDistance, anglePerSpawm, spawnIndex);
            spawnIndex++;
            return true;
        }

        private IUnit InternalSpawnUnit(UnitType type, Player controller, Vector3 position, float spawnDistance, float anglePerSpawm, int spawnIndex) {
            var pool = this._pools[type];
            var pos = CircleHelpers.GetPointOnCircle(position, spawnDistance, anglePerSpawm, spawnIndex);
            //Debug.Log("spawn position: " + pos);
            pos = Utility.Utils.GetGroundedPosition((pos) + new Vector3(0.0f, 0.5f, 0.0f));
            var unit = pool.Get(pos, Quaternion.identity);

            unit.controller = controller;
            unit.uiComponent.controller = controller;
            unit.gameObject.ColorRenderers(controller.color);

            ((UnitUI)unit.uiComponent).Init();
            controller.units.Add(unit);
            unit.transform.SetParent(controller.unitGroup.transform);

            return unit;
        }

        public void Return(IUnit unit) {
            this._pools[unit.unitType].Return(unit);
        }

        public void AddUnitDeath(GameObject go) {
            this._poolDeath.Add(new UnitDeath(go));
        }

        public void AddUnitDeath(GameObject go, int counter) {
            this._poolDeath.Add(new UnitDeath(go, counter));
        }

        public void Countdown() {
            if(this._poolDeath.Count == 0)
                return;
            else {
                List<UnitDeath> toRemove = new List<UnitDeath>();

                foreach(UnitDeath death in this._poolDeath) {
                    // count down the counter and remove when it reaches 0.
                    death.Countdown();

                    if(death.TurnCounter == 0)
                        toRemove.Add(death);
                }

                if(toRemove.Count != 0) {
                    foreach(UnitDeath remove in toRemove) {
                        if(this._poolDeath.Contains(remove))
                            this._poolDeath.Remove(remove);
                    }
                }

                toRemove.Clear();
            }
        }
    }
}