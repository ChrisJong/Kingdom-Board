namespace Player {

    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.EventSystems;

    using Constants;
    using Enum;
    using Helpers;
    using Structure;
    using UI;
    using Unit;
    using Utility;

    public class PlayerSelect : MonoBehaviour {

        #region VARIABLE

        private float _rayDistance = 50.0f;

        private bool _selected = false;

        private SelectionState _currentState = SelectionState.NONE;
        private SelectionState _previousState = SelectionState.NONE;

        private EntityType _selectedEntity = EntityType.NONE;

        [SerializeField] private HasHealthBase _currentSelected = null;
        [SerializeField] private HasHealthBase _previousSelected = null;
        [SerializeField] private HasHealthBase _currentHover = null;
        [SerializeField] private HasHealthBase _previousHover = null;

        [SerializeField] private HasHealthBase _targetSelected = null;

        [SerializeField] private Vector3 _moveTo = Vector3.zero;

        private Camera _camera;
        private Player _controller;

        private Ray _ray;
        private RaycastHit _hitInfo;

        public SelectionState CurrentState { get { return this._currentState; } set { this._previousState = this._currentState; this._currentState = value; } }
        public SelectionState PreviousState { get { return this._previousState; } }

        #endregion

        #region UNITY
        private void Update() {
            if(this._controller.CurrentState != PlayerState.DEAD)
                this.UpdatePlayerSelect();
        }
        #endregion

        #region CLASS
        public void Init(Player player) {
            if(this._camera == null) {
                if(this.GetComponent<Camera>()!= null) {

                    this._camera = this.GetComponent<Camera>() as Camera;

                } else {
                    Debug.LogError("Player Needs A Camera");
                    throw new System.NullReferenceException("There is no Camera Attached To the Player");
                }
            }

            this._controller = player;
        }

        public void EndTurn() {
            if(this._currentSelected != null)
                this._currentSelected.uiBase.HideUI();

            if(this._currentHover != null)
                this._currentHover.uiBase.OnExit();

            this._currentSelected = null;
            this._previousSelected = null;
            this._currentHover = null;
            this._previousHover = null;
        }

        public Vector3 GetCurrentPointOnGround() {
            Vector3 coord = Vector3.zero;
            Ray ray = this._camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            Physics.Raycast(ray, out hitInfo, this._rayDistance, GlobalSettings.LayerValues.groundLayer);

            coord = hitInfo.point;

            return coord;
        }

        private void UpdatePlayerSelect() {
            Debug.DrawRay(this._ray.origin, this._ray.direction * this._rayDistance, Color.yellow);
            this.MouseSelection();
        }

        private bool CastRayToWorld(LayerMask mask) {
            this._ray = this._camera.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(this._ray.origin, this._ray.direction * this._rayDistance, Color.red);

            if(EventSystem.current.IsPointerOverGameObject())
                return false;

            return Physics.Raycast(this._ray, out this._hitInfo, this._rayDistance, ~(mask));
        }

        private void MouseSelection() {
            switch(this._currentState) {
                case SelectionState.FREE:
                this.FreeState();
                break;

                case SelectionState.STANDBY:
                this.StandbyState();
                break;

                case SelectionState.SELECT_TARGET:
                this.SelectTarget();
                break;

                case SelectionState.SELECT_ALLYTARGET:
                this.SelectTarget();
                break;

                case SelectionState.SELECT_ENEMYTARGET:
                this.SelectTarget();
                break;

                case SelectionState.SELECT_POINT:
                this.SelectPoint();
                break;

                case SelectionState.SELECT_SPAWNPOINT:
                this.SelectSpawnPoint();
                break;
            }
        }

        private void FreeState() {
            if(Input.GetMouseButtonUp(0)) { // Left Mouse Click

                if(!EventSystem.current.IsPointerOverGameObject())
                    if(this._controller.castle.castleUI.SpawnGroupToggle)
                        this._controller.castle.castleUI.ToggleSpawnGroup(false);

                if(this.CastRayToWorld(GlobalSettings.LayerValues.groundLayer))
                    this.Selection();

            } else if(Input.GetMouseButtonUp(1)) { // Right M ouse Click

                if(!EventSystem.current.IsPointerOverGameObject())
                    if(this._controller.castle.castleUI.SpawnGroupToggle)
                        this._controller.castle.castleUI.ToggleSpawnGroup(false);

                this.Deselection();
            } else {
                this.HoverSelect();
            }
        }

        private void StandbyState() {
            if(Input.GetMouseButtonUp(1)) { // Right Click.

                if(!EventSystem.current.IsPointerOverGameObject())
                    if(this._controller.castle.castleUI.SpawnGroupToggle)
                        this._controller.castle.castleUI.ToggleSpawnGroup(false);

                // Castle Spawn Checks
                if(this.CastRayToWorld(GlobalSettings.LayerValues.groundLayer)) {
                    if(this._hitInfo.transform.GetComponent<HasHealthBase>() != null) {
                        HasHealthBase temp = this._hitInfo.transform.GetComponent<HasHealthBase>() as HasHealthBase;

                        if(temp.controller.id == this._controller.id) {
                            if(temp.GetType() == typeof(Castle))
                                if(!this._controller.castle.castleUI.SpawnGroupToggle)
                                    ((CastleUI)temp.uiBase).ToggleSpawnGroup(true);
                        }
                    }
                }

                if(this._selectedEntity == EntityType.UNIT) {

                } else if(this._selectedEntity == EntityType.STRUCTURE) {

                } else {
                    Debug.LogError("No Such Entity Type For the Current Selection Exsists, " + this._currentSelected.entityType.ToString());
                    throw new System.ArgumentNullException("No Such Entity Type For the Current Selection Exsists, " + this._currentSelected.entityType.ToString());
                }

                Debug.Log("Selecting Point Or Unit");

            } else if(Input.GetMouseButtonUp(0)) { // Left Click. (De-Selection)

                if(!EventSystem.current.IsPointerOverGameObject())
                    if(this._controller.castle.castleUI.SpawnGroupToggle)
                        this._controller.castle.castleUI.ToggleSpawnGroup(false);

                this.Deselection();

            } else {
                //this.HoverSelect();
            }
        }

        private void SelectPoint() {
            if(Input.GetMouseButtonUp(0)) {
                if(this.CastRayToWorld(GlobalSettings.LayerValues.unitLayer)) {

                    if(this._currentSelected is ISelected) {
                        if(!(this._currentSelected as ISelected).SetPoint(this._hitInfo.point)) {
                            Debug.LogWarning("The Location Selected Is invalid: " + this._hitInfo.point.ToString());
                            return;
                        }
                    } else {
                        Debug.LogWarning(this._currentSelected.name + " Doesn't Inherit from ISelection Interface.");
                    }
                }
            }
        }

        private void SelectTarget() {
            if(Input.GetMouseButtonUp(0)) {
                if(this.CastRayToWorld(GlobalSettings.LayerValues.groundLayer)) {
                    if(this._hitInfo.transform.GetComponent<HasHealthBase>() != null) {
                        _targetSelected = this._hitInfo.transform.GetComponent<HasHealthBase>();

                        if(this.CheckTargetSelection(_targetSelected)) {
                            if(this._currentSelected is ISelected) {
                                if(!(this._currentSelected as ISelected).SetTarget(_targetSelected as IHasHealth)) {
                                    Debug.Log("Unit out of range");
                                    return;
                                }
                            } else {
                                Debug.LogWarning(this._currentSelected.name + " Deson't Inherit from ISelection Interface.");
                            }
                        } else {
                            Debug.LogWarning(_targetSelected.name + " - Entity Isn't Apart Of Any Player Group.");
                            return;
                        }
                    }
                }
            }
        }

        // NOTE: this needs to be changed let the unit/structure figure out if the targetselected needs to be an ally or an enemy.
        private bool CheckTargetSelection(HasHealthBase target) {
            if(this._currentState == SelectionState.SELECT_ENEMYTARGET) {
                return this._currentSelected.IsEnemy(target);
            } else if(this._currentState== SelectionState.SELECT_ALLYTARGET) {
                return this._currentSelected.IsAlly(target);
            } else if(this._currentState == SelectionState.SELECT_TARGET) {
                return true;
            } else {
                return false;
            }
        }

        private void SelectSpawnPoint() {
            if(Input.GetMouseButtonUp(0)) { // Left Click!

                if(this.CastRayToWorld(GlobalSettings.LayerValues.unitLayer)) {

                    if(this._controller.castle != null) {

                        if(!this._controller.castle.SetPoint(this._hitInfo.point)) {
                            Debug.LogWarning("The Location Selected Is Invalud: " + this._hitInfo.point.ToString());
                            return;
                        }

                    } else {
                        Debug.LogError("Player_" + this._controller.id.ToString() + "Doesn't Contain A Castle Object");
                    }
                }

            } else if(Input.GetMouseButtonUp(1)) { // Right Click!

                if(this._controller.castle != null) {

                    this._controller.castle.CancelSpawn();

                } else {
                    Debug.LogError("Player_" + this._controller.id.ToString() + "Doesn't Contain A Castle Object");
                }

                this._previousState = this._currentState;
                this._currentState = SelectionState.FREE;

            }
        }

        private void Selection() {
            if(this._hitInfo.transform.GetComponent<HasHealthBase>() != null) {
                var temp = this._hitInfo.transform.GetComponent<HasHealthBase>();

                if(this._controller.CurrentState == PlayerState.DEFENDING && temp.entityType == EntityType.UNIT)
                    return;

                if(temp.entityType == EntityType.UNIT) {
                    if(((UnitBase)temp).unitState == UnitState.FINISH)
                        return;
                }

                if(temp.controller.id == this._controller.id) {
                    if(this._currentSelected != null && !temp.Equals(this._currentSelected)) {
                        this._previousSelected = this._currentSelected;
                        this._previousSelected.uiBase.HideUI();
                    }

                    this._currentSelected = temp;
                    this._currentSelected.uiBase.DisplayUI();

                    this._previousState = this._currentState;
                    this._currentState = SelectionState.STANDBY;
                    this._selectedEntity = temp.entityType;
                    this._selected = true;
                }
            } else {
                Debug.LogWarning(this._hitInfo.transform.name + " Doesn't Derive From HasHealthBase Class.");
            }
        }

        private void Deselection() {
            if(!EventSystem.current.IsPointerOverGameObject()) {
                if(this._currentSelected != null) {
                    this._previousSelected = this._currentSelected;
                    this._currentSelected.uiBase.HideUI(); // Hide UI;
                    this._currentSelected = null;
                }

                this._previousState = this._currentState;
                this._currentState = SelectionState.FREE;
                this._selected = false;
            }
        }

        private void HoverSelect() {
            if(this.CastRayToWorld(GlobalSettings.LayerValues.groundLayer)) {
                if(this._hitInfo.transform.GetComponent<HasHealthBase>() != null) {
                    if(this._currentHover != null) {

                        HasHealthBase temp = this._hitInfo.transform.GetComponent<HasHealthBase>() as HasHealthBase;

                        if(this._currentHover == temp)
                            return;

                        this._previousHover = this._currentHover;
                        this._previousHover.uiBase.OnExit();
                        this._currentHover = this._hitInfo.transform.GetComponent<HasHealthBase>() as HasHealthBase;
                        this._currentHover.uiBase.OnEnter();
                    } else {
                        this._currentHover = this._hitInfo.transform.GetComponent<HasHealthBase>() as HasHealthBase;
                        this._currentHover.uiBase.OnEnter();
                    }
                }
            } else {
                if(this._currentHover != null) {
                    this._previousHover = this._currentHover;
                    this._previousHover.uiBase.OnExit();
                }

                this._currentHover = null;
            }
        }

        #endregion
    }
}