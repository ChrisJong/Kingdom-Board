namespace KingdomBoard.Manager {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;
    using Extension;
    using Helpers;
    using Player;
    using Scriptable;
    using Unit;
    using Utility;

    public sealed class UnitPoolManager : SingletonMono<UnitPoolManager> {

        #region VARIABLE

        private static readonly int unitTypeLength = System.Enum.GetNames(typeof(UnitType)).Length - 2;
        private readonly int _unitInstanceCount = 5;
        private readonly int _placementInstanceCount = 1;
        private readonly int _deathInstanceCount = 1;

        private readonly Dictionary<UnitType, UnitPool> _unitPools = new Dictionary<UnitType, UnitPool>(unitTypeLength, new UnitTypeComparer());
        private readonly Dictionary<UnitType, UnitPlacementPool> _placementPools = new Dictionary<UnitType, UnitPlacementPool>(unitTypeLength, new UnitTypeComparer());
        private readonly Dictionary<UnitType, UnitDeathPool> _deathPools = new Dictionary<UnitType, UnitDeathPool>(unitTypeLength, new UnitTypeComparer());

        [SerializeField] private List<IUnitDeath> _poolDeath = new List<IUnitDeath>();

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

                foreach(UnitScriptable data in this._unitDataList) {

                    GameObject host = new GameObject(data.unitType.ToString());
                    GameObject placementHost = new GameObject(data.unitType.ToString() + "_Placement");
                    GameObject deathHost = new GameObject(data.unitType.ToString() + "_Death");
                    host.transform.SetParent(managerHost.transform);
                    placementHost.transform.SetParent(managerHost.transform);
                    deathHost.transform.SetParent(managerHost.transform);

                    if(data.mainPrefab == null) {
                        Debug.LogError("Unit Scriptable(Data) of type (" + data.unitType.ToString() + ") doesn't contain a main prefab, please attach one!");
                        throw new System.NullReferenceException("Unit Scriptable(Data) of type (" + data.unitType.ToString() + ") doesn't contain a main prefab, please attach one!");
                    }
                    GameObject unitPrefab = data.mainPrefab;
                    UnitBase unitBase = unitPrefab.GetComponent<UnitBase>();
                    if(unitBase != null) {
                        if(unitBase.SetData(data)){
                            unitBase.Setup();
                            unitBase.SetupAnimation();
                            this._unitPools.Add(data.unitType, new UnitPool(unitPrefab, host, this._unitInstanceCount));
                        } else {
                            Debug.LogError("Data for the unit type doesn't exists! " + data.unitType.ToString());
                            throw new System.ArgumentNullException("Data for the unit type doesn't exists! " + data.unitType.ToString());
                        }
                    } else {
                        Debug.LogError("Unit prefab doesn't contain the unitbase/unittype script! " + data.unitType.ToString());
                        throw new System.ArgumentException("Unit prefab doesn't contain the unitbase/unittype script! " + data.unitType.ToString());
                    }

                    if(data.placementPrefab == null) {
                        Debug.LogError("Unit Scriptable(Data) of type (" + data.unitType.ToString() + ") doesn't contain a placement prefab, please attach one!");
                        throw new System.NullReferenceException("Unit Scriptable(Data) of type (" + data.unitType.ToString() + ") doesn't contain a placement prefab, please attach one!");
                    }
                    GameObject placementPrefab = data.placementPrefab;
                    UnitPlacement placementBase = placementPrefab.GetComponent<UnitPlacement>();
                    if(placementBase == null)
                        placementBase = placementPrefab.AddComponent<UnitPlacement>();
                    placementBase.unitType = data.unitType;
                    this._placementPools.Add(data.unitType, new UnitPlacementPool(placementPrefab, placementHost, this._placementInstanceCount));

                    if(data.deathPrefab == null) {
                        Debug.LogError("Unit Scriptable(Data) of type (" + data.unitType.ToString() + ") doesn't contain a death prefab, please attach one!");
                        throw new System.NullReferenceException("Unit Scriptable(Data) of type (" + data.unitType.ToString() + ") doesn't contain a death prefab, please attach one!");
                    }
                    GameObject deathPrefab = data.deathPrefab;
                    UnitDeath deathBase = deathPrefab.GetComponent<UnitDeath>();
                    if(deathBase == null)
                        deathBase = deathPrefab.AddComponent<UnitDeath>();
                    deathBase.unitType = data.unitType;
                    this._deathPools.Add(data.unitType, new UnitDeathPool(deathPrefab, deathHost, this._deathInstanceCount));
                }
            }
        }

        public bool SpawnUnit(UnitType type, Player controller, Vector3 position) {

            if(type == UnitType.NONE || type == UnitType.ANY || !this._unitPools.ContainsKey(type)) {
                Debug.LogError(this.ToString() + " cannot spawn unit of type (not suypported)" + type);
                return false;
            }

            this.InternalSpawnUnit(type, controller, position);

            return true;
        }

        public bool SpawnUnit(UnitType type, Player controller, Vector3 position, Vector3 structurePosition) {

            if(type == UnitType.NONE || type == UnitType.ANY || !this._unitPools.ContainsKey(type)) {
                Debug.LogError(this.ToString() + " cannot spawn unit of type (not suypported)" + type);
                return false;
            }

            IUnit temp = this.InternalSpawnUnit(type, controller, position);
            Vector3 direction = position - structurePosition;
            temp.rotation = Quaternion.LookRotation(direction, Vector3.up);

            return true;
        }

        public bool SpawnUnit(UnitType type, Player controller, Vector3 position, float spawnDistance, float anglePerSpawm, ref uint spawnIndex) {
            if(type == UnitType.NONE || type == UnitType.ANY || !this._unitPools.ContainsKey(type)) {
                Debug.LogError(this.ToString() + " cannot spawn unit of type (not supported): " + type);
                return false;
            }
            this.InternalSpawnUnit(type, controller, position, spawnDistance, anglePerSpawm, spawnIndex);
            spawnIndex++;
            return true;
        }

        public bool SpawnPlacement(UnitType type, Vector3 position, out UnitPlacement plavement) {
            if(type == UnitType.NONE || type == UnitType.ANY || !this._placementPools.ContainsKey(type)) {
                Debug.LogError(this.ToString() + " cannot spawn unit of type (not supported): " + type);
                plavement = null;
                return false;
            }

            plavement = this.InternalSpawnPlavement(type, position) as UnitPlacement;
            return true;
        }

        public bool SpawnDeath(Color color, UnitType type, Vector3 position, Quaternion rotation, Vector3 eDirection, Vector3 ePosition, float eForce, int counter) {

            if(type == UnitType.NONE || type == UnitType.ANY || !this._placementPools.ContainsKey(type)) {
                Debug.LogError(this.ToString() + " cannot spawn unit of type (not supported): " + type);
                return false;
            }
            
            IUnitDeath temp = this.InternalSpawnDeath(type, position, rotation);

            if(temp == null)
                return false;

            temp.Init(color, eDirection, ePosition, eForce, counter);

            this._poolDeath.Add(temp);

            return true;
        }

        public void Return(IUnit unit) {
            unit.Return();
            this._unitPools[unit.unitType].Return(unit);
        }

        public void Return(IUnitPlacement placement) {
            placement.Return();
            this._placementPools[placement.unitType].Return(placement);
        }

        public void Return(IUnitDeath death) {
            death.Return();
            this._deathPools[death.unitType].Return(death);
        }

        public void CountdownDeath() {
            if(this._poolDeath.Count <= 0)
                return;

            List<IUnitDeath> toRemove = new List<IUnitDeath>();

            foreach(IUnitDeath death in this._poolDeath) {
                // count down the counter and remove when it reaches 0.
                death.Countdown();

                if(death.TurnCounter == 0)
                    toRemove.Add(death);
            }

            if(toRemove.Count != 0) {
                foreach(IUnitDeath remove in toRemove) {
                    if(this._poolDeath.Contains(remove)) {
                        this.Return(remove);
                        this._poolDeath.Remove(remove);
                    }
                }
            }

            toRemove.Clear();
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

        private IUnit InternalSpawnUnit(UnitType type, Player controller, Vector3 position) {
            UnitPool pool = this._unitPools[type];
            Vector3 pos = position;
            pos = Utils.GetGroundedPosition((pos) + new Vector3(0.0f, 0.5f, 0.0f));
            IUnit unit = pool.Get(pos, Quaternion.identity);

            if(!unit.IsSetup)
                unit.Setup();

            unit.Init(controller);
            unit.gameObject.ColorRenderers(controller.PlayerColor);

            controller.AddUnit(unit);
            unit.transform.SetParent(controller.UnitGroup.transform);

            return unit;
        }

        private IUnit InternalSpawnUnit(UnitType type, Player controller, Vector3 position, float spawnDistance, float anglePerSpawm, uint spawnIndex) {
            UnitPool pool = this._unitPools[type];
            Vector3 pos = CircleHelpers.GetPointOnCircle(position, spawnDistance, anglePerSpawm, spawnIndex);
            pos = Utility.Utils.GetGroundedPosition((pos) + new Vector3(0.0f, 0.5f, 0.0f));
            IUnit unit = pool.Get(pos, Quaternion.identity);

            if(!unit.IsSetup)
                unit.Setup();

            unit.Init(controller);
            unit.gameObject.ColorRenderers(controller.PlayerColor);

            controller.AddUnit(unit);
            unit.transform.SetParent(controller.UnitGroup.transform);

            return unit;
        }

        private IUnitPlacement InternalSpawnPlavement(UnitType type, Vector3 position) {
            UnitPlacementPool pool = this._placementPools[type];
            Vector3 pos = position;
            pos = Utils.GetGroundedPosition(pos + new Vector3(0.0f, 0.5f, 0.0f));
            IUnitPlacement plavement = pool.Get(pos, Quaternion.identity);

            if(!plavement.IsSetup)
                plavement.Setup();

            return plavement;
        }

        private IUnitDeath InternalSpawnDeath(UnitType type, Vector3 position, Quaternion rotation) {
            UnitDeathPool pool = this._deathPools[type];
            Vector3 pos = position;
            pos = Utils.GetGroundedPosition(pos + new Vector3(0.0f, 0.5f, 0.0f));
            IUnitDeath death = pool.Get(pos, rotation);

            if(!death.IsSetup)
                death.Setup();

            return death;
        }

        #endregion
    }
}