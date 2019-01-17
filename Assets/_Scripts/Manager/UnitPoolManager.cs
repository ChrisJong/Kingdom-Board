namespace Manager {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;
    using Extension;
    using Helpers;
    using Player;
    using Scriptable;
    using UI;
    using Unit;
    using Utility;

    public sealed class UnitPoolManager : SingletonMono<UnitPoolManager> {

        #region VARIABLE

        private static readonly int unitTypeLength = System.Enum.GetNames(typeof(UnitType)).Length - 2;

        [SerializeField] private UnitPoolSetup[] _poolSetup = new UnitPoolSetup[unitTypeLength];
        private readonly Dictionary<UnitType, UnitPool> _pools = new Dictionary<UnitType, UnitPool>(unitTypeLength, new UnitTypeComparer());

        [SerializeField] private List<UnitDeath> _poolDeath = new List<UnitDeath>();

        private List<UnitScriptable> _unitDataList = new List<UnitScriptable>();
        private Dictionary<UnitClassType, List<UnitScriptable>> _sortedUnitClass = new Dictionary<UnitClassType, List<UnitScriptable>>();
        private Dictionary<UnitType, UnitScriptable> _sortedUnitData = new Dictionary<UnitType, UnitScriptable>();

        public List<UnitScriptable> UnitDataList { get { return this._unitDataList; } }
        public Dictionary<UnitClassType, List<UnitScriptable>> SortedUnitClass { get { return this._sortedUnitClass; } }
        public Dictionary<UnitType, UnitScriptable> SortedUnitData { get { return this._sortedUnitData; } }

        #endregion

        #region UNITY
        protected override void Awake() {
            base.Awake();

            this.Init();
        }
        #endregion

        #region CLASS
        public override void Init() {
            if(this.LoadUnitData()) {

                var managerHost = new GameObject("Units");
                managerHost.transform.SetParent(this.transform);

                for(int i = 0; i < this._poolSetup.Length; i++) {
                    UnitPoolSetup setup = this._poolSetup[i];

                    GameObject host = new GameObject(setup.type.ToString());
                    host.transform.SetParent(managerHost.transform);

                    setup.prefab.GetComponent<UnitBase>().UnitData = this._sortedUnitData[setup.type];

                    this._pools.Add(setup.type, new UnitPool(setup.prefab, host, setup.initialInstanceCount));

                    setup.prefab.GetComponent<UnitBase>().SetupAnimation();
                }
            }
        }

        public bool SpawnUnit(UnitType type, Player controller, Vector3 position) {

            if(type == UnitType.NONE || type == UnitType.ANY || !this._pools.ContainsKey(type)) {
                Debug.LogError(this.ToString() + " cannot spawn unit of type (not suypported)" + type);
                return false;
            }

            this.InteralSpawnUnit(type, controller, position);

            return true;
        }

        public bool SpawnUnit(UnitType type, Player controller, Vector3 position, float spawnDistance, float anglePerSpawm, ref uint spawnIndex) {
            if(type == UnitType.NONE || type == UnitType.ANY || !this._pools.ContainsKey(type)) {
                Debug.LogError(this.ToString() + " cannot spawn unit of type (not supported): " + type);
                return false;
            }
            this.InternalSpawnUnit(type, controller, position, spawnDistance, anglePerSpawm, spawnIndex);
            spawnIndex++;
            return true;
        }
        
        private bool LoadUnitData() {

            int classCount = System.Enum.GetNames(typeof(UnitClassType)).Length - 2;

            for(int i = 0; i < classCount; i++) {
                List<UnitScriptable> tempList = new List<UnitScriptable>();
                string className = ((UnitClassType)i + 1).ToString();
                string path = "Scriptable/Units/" + className;

                UnityEngine.Object[] temp = Resources.LoadAll(path, typeof(UnitScriptable));

                for(int a = 0; a < temp.Length; a++)
                    tempList.Add(temp[a] as UnitScriptable);

                tempList.Sort((x1, x2) => x1.classTier.CompareTo(x2.classTier));

                this._sortedUnitClass.Add(((UnitClassType)i + 1), tempList);

                foreach(UnitScriptable data in tempList) {
                    this._sortedUnitData.Add(data.unitType, data);
                }

                this._unitDataList.AddRange(tempList);

            }

            if(this._unitDataList.Count > 0)
                return true;
            else
                return false;
        }

        private IUnit InteralSpawnUnit(UnitType type, Player controller, Vector3 position) {
            UnitPool pool = this._pools[type];
            Vector3 pos = position;
            pos = Utils.GetGroundedPosition((pos) + new Vector3(0.0f, 0.5f, 0.0f));
            IUnit unit = pool.Get(pos, Quaternion.identity);

            if(!unit.IsSetup) {
                unit.Setup();
            }

            unit.Init(controller);
            unit.gameObject.ColorRenderers(controller.color);

            controller.AddUnit(unit);
            unit.transform.SetParent(controller.unitGroup.transform);

            return unit;
        }

        private IUnit InternalSpawnUnit(UnitType type, Player controller, Vector3 position, float spawnDistance, float anglePerSpawm, uint spawnIndex) {
            UnitPool pool = this._pools[type];
            Vector3 pos = CircleHelpers.GetPointOnCircle(position, spawnDistance, anglePerSpawm, spawnIndex);
            pos = Utility.Utils.GetGroundedPosition((pos) + new Vector3(0.0f, 0.5f, 0.0f));
            IUnit unit = pool.Get(pos, Quaternion.identity);

            if(!unit.IsSetup) {
                unit.Setup();
            }

            unit.Init(controller);
            unit.gameObject.ColorRenderers(controller.color);

            controller.AddUnit(unit);
            unit.transform.SetParent(controller.unitGroup.transform);

            return unit;
        }

        public void Return(IUnit unit) {
            unit.Return();
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

        public UnitScriptable FetchUnitData(UnitType unitType) {
            UnitScriptable unitData = null;

            for(int i = 0; i < this._unitDataList.Count; i++) {
                if(this._unitDataList[i].unitType == unitType) {
                    unitData = this._unitDataList[i];
                    break;
                }
            }

            if(unitData == null)
                Debug.LogError("No Such Unit (" + unitType.ToString() + ")");

            return unitData;
        }

        public UnitScriptable FetchUnitData(UnitClassType classType, UnitType unitType) {
            List<UnitScriptable> unitList = this.FetchClassUnitsData(classType);
            UnitScriptable unitData = null;

            if(unitList == null || unitList.Count == 0)
                Debug.LogError("No Such Unit (" + unitType.ToString() + ") Exists within the " + classType.ToString() + " Class");
            else {
                for(int i = 0; i < unitList.Count; i++) {
                    if(unitList[i].unitType == unitType) {
                        unitData = unitList[i];
                        break;
                    }
                }
            }

            if(unitData == null)
                Debug.LogError("No Such Unit (" + unitType.ToString() + ") Exists within the " + classType.ToString() + " Class");

            return unitData;
        }

        public List<UnitScriptable> FetchClassUnitsData(UnitClassType classType) {
            if(this._sortedUnitClass.ContainsKey(classType))
                return this._sortedUnitClass[classType];
            else {
                Debug.LogError("There Is No Such Class Type Called: " + classType.ToString());
                return null;
            }
        }

        #endregion
    }
}