namespace KingdomBoard.Structure {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.AI;

    using Enum;
    using Helpers;
    using Manager;
    using Player;

    [RequireComponent(typeof(NavMeshObstacle))]
    public class GoldMine : EntityBase {

        #region VARIABLE

        [SerializeField] private bool _inControl = false;

        [SerializeField] private int _gold = 1000;

        [SerializeField] private float _radiusCheck = 3.0f;

        [SerializeField] private Player _playerInControl = null;

        private Dictionary<Player, int> _playerUnitCount = new Dictionary<Player, int>();

        [SerializeField] private List<HasHealthBase> _entitiesNear = new List<HasHealthBase>();

        private SphereCollider _triggerCollider = null;

        private NavMeshObstacle _navObstacle = null;

        [SerializeField] private List<Animation> _flagAnimation = new List<Animation>();
        [SerializeField] private List<SkinnedMeshRenderer> _flagRenderer = new List<SkinnedMeshRenderer>();
        [SerializeField] private List<Material> _flagMaterial = new List<Material>(); // Element 0 is White, Element 1 is Black.

        public StructureClassType classType { get { return StructureClassType.NEUTRAL; } }
        public StructureType structureType { get { return StructureType.GOLDMINE; } }
        public override EntityType entityType { get { return EntityType.STRUCTURE; } }

        public int Gold { get { return this._gold; } }
        public bool InControl { get { return this._inControl; } }
        public Player PlayerInControl { get { return this._playerInControl; } }

        #endregion

        #region UNITY

        private void OnTriggerEnter(Collider other) {

            HasHealthBase temp = other.GetComponent<HasHealthBase>() as HasHealthBase;
            if(temp != null) {
                this.AddEntity(temp);
            }
        }

        private void OnTriggerExit(Collider other) {

            HasHealthBase temp = other.GetComponent<HasHealthBase>() as HasHealthBase;
            if(temp != null) {
                this.RemoveEntity(temp);
            }
        }

        #endregion

        #region CLASS
        public override void Setup() {

            this._entitiesNear.Clear();

            this._triggerCollider = this.transform.GetComponent<SphereCollider>() as SphereCollider;
            this._navObstacle = this.transform.GetComponent<NavMeshObstacle>() as NavMeshObstacle;

            if(this._flagMaterial.Count == 0) {
                Debug.LogError("No Team Materials found for the flag.");
                return;
            }

            if(this._flagAnimation.Count == 0) {
                Animation[] animation = this.transform.GetComponentsInChildren<Animation>();

                if(animation.Length == 0) {
                    Debug.LogError("No Animation Components Found!");
                    return;
                }

                this._flagAnimation.AddRange(animation);
            }

            if(this._flagRenderer.Count == 0) {
                SkinnedMeshRenderer[] skinned = this.transform.GetComponentsInChildren<SkinnedMeshRenderer>();

                if(skinned.Length == 0) {
                    Debug.LogError("No Skinned Mesh Renderere Found!");
                    return;
                }

                this._flagRenderer.AddRange(skinned);
            }
        }

        public override void Init() {
            this._triggerCollider.radius = this._radiusCheck;
            this._triggerCollider.isTrigger = true;
            this._navObstacle.carving = true;

            this._inControl = false;
            this._playerInControl = null;
        }

        public void Init(List<Player> players) {

            foreach(Player p in players)
                this._playerUnitCount.Add(p, 0);

            this.Init();
        }

        public override void Return() {

            this._playerInControl = null;

            this._entitiesNear.Clear();
            this._playerUnitCount.Clear();
        }

        public void CheckControl() {

            bool controlStatus = false;
            Player control = null;

            if(this._entitiesNear.Count <= 0) {
                if(this._inControl)
                    DestroyFlags();

                this._inControl = controlStatus;
                this._playerInControl = control;
                return;
            }

            foreach(Player p in this._playerUnitCount.Keys) {

                if(this._playerUnitCount[p] <= 0) // If there are no units for the specific player then continue to the next player.
                    continue;

                if(controlStatus) { // Gold Mine is being contested.
                    controlStatus = false;
                    control = null;
                    if(this._playerInControl != null)
                        DestroyFlags();
                    break;
                }

                // A Player is in control of the gold mine for now
                controlStatus = true;
                control = p;
            }

            if(control != null && this._playerInControl != null) {
                if(control != this._playerInControl)
                    SpawnFlags(control);
            } else if(control != null && this._playerInControl == null) {
                SpawnFlags(control);
            }

            this._inControl = controlStatus;
            this._playerInControl = control;
        }

        public void AddEntity(HasHealthBase entity) {

            if(this._entitiesNear.Contains(entity))
                return;

            this._entitiesNear.Add(entity);

            this._playerUnitCount[entity.Controller] += 1;

            /*if(entity.controller == this._playerInControl)
                return;*/

            this.CheckControl();
        }

        public void RemoveEntity(HasHealthBase entity) {

            if(!this._entitiesNear.Contains(entity))
                return;

            if(this._entitiesNear.Count < 0) {
                Debug.LogError("Entity Count Near the Mine Exceeds Negative 0: " + this._entitiesNear.Count.ToString());
                return;
            }

            this._entitiesNear.Remove(entity);

            if(this._playerUnitCount[entity.Controller] < 0) {
                Debug.LogError(entity.Controller.name + " Entity Count Near the Mine Exceeds Negative 0: " + this._playerUnitCount[entity.Controller].ToString());
                return;
            }
            this._playerUnitCount[entity.Controller] -= 1;

            this.CheckControl();
        }

        private void SpawnFlags(Player control) {

            this.ChangeColor(control);

            for(int i = 0; i < this._flagAnimation.Count; i++) {
                if(this._flagAnimation[i].isPlaying)
                    this._flagAnimation[i].Blend("flag_Spawn");
                else
                    this._flagAnimation[i].Play("flag_Spawn");
            }
        }

        private void DestroyFlags() {
            for(int i = 0; i < this._flagAnimation.Count; i++) {
                if(this._flagAnimation[i].isPlaying)
                    this._flagAnimation[i].Blend("flag_Destroy");
                else
                    this._flagAnimation[i].Play("flag_Destroy");
            }
        }

        private void ChangeColor(Player control) {

            if(control.PlayerColor.Equals(Constants.PlayerValues.WHITE)) {
                foreach(SkinnedMeshRenderer skin in this._flagRenderer)
                    skin.material = this._flagMaterial[0];
            } else {
                foreach(SkinnedMeshRenderer skin in this._flagRenderer)
                    skin.material = this._flagMaterial[1];
            }
        }
        #endregion

    }
}