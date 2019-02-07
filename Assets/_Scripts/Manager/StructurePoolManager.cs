namespace Manager {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;
    using Extension;
    using Player;
    using Structure;
    using Utility;

    public sealed class StructurePoolManager : SingletonMono<StructurePoolManager> {

        private static readonly int structureTypeLength = System.Enum.GetNames(typeof(StructureType)).Length - 2;

        [SerializeField] private StructurePoolSetup[] _poolSetup = new StructurePoolSetup[structureTypeLength];
        private readonly Dictionary<StructureType, StructurePool> _pools = new Dictionary<StructureType, StructurePool>(structureTypeLength, new StructureTypeComparer());

        private List<StructureScriptable> _structureDataList = new List<StructureScriptable>();
        private Dictionary<StructureType, StructureScriptable> _sortedStructureData = new Dictionary<StructureType, StructureScriptable>();

        protected override void Awake() {
            base.Awake();

            this.Init();
        }

        public override void Init() {
            if(this.LoadStructureData()) {

                var managerHost = new GameObject("Structures");
                managerHost.transform.SetParent(this.transform);

                for(int i = 0; i < this._poolSetup.Length; i++) {
                    StructurePoolSetup setup = this._poolSetup[i];

                    GameObject host = new GameObject(setup.type.ToString());
                    host.transform.SetParent(this.transform);

                    if(setup.prefab.GetComponent<StructureBase>().SetData(this._sortedStructureData[setup.type])) {
                        setup.prefab.GetComponent<StructureBase>().Setup();
                    } else {
                        Debug.LogError("Data For the Structure Type Doesn't Exist: " + setup.type.ToString());
                        throw new System.ArgumentNullException("Data For the Structure Type Doesn't Exist: " + setup.type.ToString());
                    }

                    this._pools.Add(setup.type, new StructurePool(setup.prefab, host, setup.initialInstanceCount));
                }
            }
        }

        public Castle GetStartCastle(Player player) {
            return player.castle ?? (Castle)InternalBuild(StructureType.CASTLE, player, player.SpawnLocation.position);
        }

        private IStructure InternalBuild(StructureType type, Player controller, Vector3 position) {
            StructurePool pool = this._pools[type];

            Vector3 pos = Utils.GetGroundedPosition(position);
            IStructure structure = pool.Get(pos, controller.SpawnLocation.rotation);

            if(!structure.IsSetup)
                structure.Setup();

            structure.Init(controller);
            structure.gameObject.ColorRenderers(controller.PlayerColor);

            controller.structures.Add(structure);
            structure.transform.SetParent(controller.StructureGroup.transform);
            return structure;
        }

        public void Return(IStructure structure) {
            this._pools[structure.structureType].Return(structure);
        }

        public bool LoadStructureData() {

            int classCount = System.Enum.GetNames(typeof(StructureClassType)).Length - 2;

            for(int i = 0; i < classCount; i++) {

                List<StructureScriptable> tempList = new List<StructureScriptable>();
                string className = ((StructureClassType)i + 1).ToString();
                string path = "Scriptable/Structures/" + className;

                UnityEngine.Object[] temp = Resources.LoadAll(path, typeof(StructureScriptable));

                for(int a = 0; a < temp.Length; a++)
                    tempList.Add(temp[a] as StructureScriptable);

                foreach(StructureScriptable data in tempList) {
                    this._sortedStructureData.Add(data.structureType, data);
                }

                this._structureDataList.AddRange(tempList);

            }


            if(this._structureDataList.Count > 0)
                return true;
            else
                return false;
            
        }
    }

}