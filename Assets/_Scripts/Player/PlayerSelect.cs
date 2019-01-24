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

        [SerializeField] private SelectionState _currentState = SelectionState.NONE;
        [SerializeField] private SelectionState _previousState = SelectionState.NONE;

        [SerializeField] private EntityType _currentSelectedEntity = EntityType.NONE;

        [Space]
        [SerializeField] private HasHealthBase _currentSelected = null;
        [SerializeField] private HasHealthBase _previousSelected = null;
        [SerializeField] private HasHealthBase _currentHover = null;
        [SerializeField] private HasHealthBase _previousHover = null;

        [SerializeField] private UnitBase _currentUnitSelected = null; // _currentSelectted is type of Unit.
        [SerializeField] private StructureBase _currentStructureSelected = null; // _currentselected is type of structure.

        [Space]
        [SerializeField] private HasHealthBase _targetSelected = null;
        [SerializeField] private Vector3 _moveTo = Vector3.zero;

        private Camera _camera = null;
        private Player _controller = null;
        private Human _humanController = null;

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

            if(player.GetType() == typeof(Human))
                this._humanController = player as Human;
            this._controller = player;
        }

        public void EndTurn() {
            if(this._currentSelected != null)
                this._currentSelected.uiBase.Hide();

            if(this._currentHover != null)
                this._currentHover.uiBase.OnExit();

            this._currentSelected = null;
            this._previousSelected = null;
            this._currentHover = null;
            this._previousHover = null;
        }

        private void UpdatePlayerSelect() {
            Debug.DrawRay(this._ray.origin, this._ray.direction * this._rayDistance, Color.yellow);
            this.MouseSelection();
        }

        private Vector3 GetCurrentPointOnGround() {
            Vector3 coord = Vector3.zero;
            Ray ray = this._camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            Physics.Raycast(ray, out hitInfo, this._rayDistance, GlobalSettings.LayerValues.groundLayer);

            coord = hitInfo.point;

            return coord;
        }

        private bool CastRayToWorld() {
            this._ray = this._camera.ScreenPointToRay(Input.mousePosition);

            //Debug.DrawRay(this._ray.origin, this._ray.direction * this._rayDistance, Color.blue);

            if(EventSystem.current.IsPointerOverGameObject())
                return false;

            return Physics.Raycast(this._ray, out this._hitInfo, this._rayDistance);
        }

        private bool CastRayToWorldToMask(LayerMask mask) {
            this._ray = this._camera.ScreenPointToRay(Input.mousePosition);

            //Debug.DrawRay(this._ray.origin, this._ray.direction * this._rayDistance, Color.red);

            if(EventSystem.current.IsPointerOverGameObject())
                return false;

            return Physics.Raycast(this._ray, out this._hitInfo, this._rayDistance, mask);
        }

        private bool CastRayToWorldIgnoreMask(LayerMask ignoreMask) {
            this._ray = this._camera.ScreenPointToRay(Input.mousePosition);

            //Debug.DrawRay(this._ray.origin, this._ray.direction * this._rayDistance, Color.red);

            if(EventSystem.current.IsPointerOverGameObject())
                return false;

            return Physics.Raycast(this._ray, out this._hitInfo, this._rayDistance, ~(ignoreMask));
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

                if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.groundLayer))
                    this.Selection();

            } else if(Input.GetMouseButtonUp(1)) { // Right Mouse Click

                if(!EventSystem.current.IsPointerOverGameObject())
                    if(this._controller.castle.castleUI.SpawnGroupToggle)
                        this._controller.castle.castleUI.ToggleSpawnGroup(false);

            } else
                this.FreeHover();
        }

        private void StandbyState() {

            if(Input.GetMouseButtonUp(1)) { // Right Click.

                if(!EventSystem.current.IsPointerOverGameObject())
                    if(this._controller.castle.castleUI.SpawnGroupToggle)
                        this._controller.castle.castleUI.ToggleSpawnGroup(false);

                // Cast A Ray Ignoring the ground layer.
                if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.groundLayer)) {

                    HasHealthBase temp = this._hitInfo.transform.GetComponent<HasHealthBase>() as HasHealthBase;

                    // Hit Either a unit or structure
                    if(temp != null) {

                    }

                } else {
                    // Hit nothing but air, soo check for the ground.
                    if(this._currentSelectedEntity == EntityType.UNIT) {

                        if(this.CastRayToWorldToMask(GlobalSettings.LayerValues.groundLayer))
                            this._currentUnitSelected.Move();

                    }
                }


                /*if(this._currentSelectedEntity == EntityType.UNIT) {



                } else if(this._currentSelectedEntity == EntityType.STRUCTURE) {

                } else {
                    Debug.LogError("No Such Entity Type For the Current Selection Exsists, " + this._currentSelected.entityType.ToString());
                    throw new System.ArgumentNullException("No Such Entity Type For the Current Selection Exsists, " + this._currentSelected.entityType.ToString());
                }*/

            } else if(Input.GetMouseButtonUp(0)) { // Left Click. (De-Selection)

                if(!EventSystem.current.IsPointerOverGameObject())
                    if(this._controller.castle.castleUI.SpawnGroupToggle)
                        this._controller.castle.castleUI.ToggleSpawnGroup(false);

                if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.groundLayer))
                    this.Selection();
                else
                    this.Deselection();

            } else {
                this.StandbyHover();
            }
        }

        private void SelectPoint() {
            if(Input.GetMouseButtonUp(0)) {
                if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.unitLayer)) {

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
                if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.groundLayer)) {
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

                if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.unitLayer)) {

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
                 HasHealthBase temp = this._hitInfo.transform.GetComponent<HasHealthBase>();

                if(this._controller.CurrentState == PlayerState.DEFENDING && temp.entityType == EntityType.UNIT)
                    return;

                if(temp.IsEnemy(this._controller))
                    return;

                if(temp.entityType == EntityType.UNIT) {
                    if(((UnitBase)temp).unitState == UnitState.FINISH)
                        return;
                }

                if(temp.controller.id == this._controller.id) {

                    if(this._currentSelected != null && !temp.Equals(this._currentSelected)) {
                        this._previousSelected = this._currentSelected;
                        this._previousSelected.uiBase.Hide();
                    }

                    this._currentSelected = temp;
                    this._currentSelected.uiBase.Display();
                    this._currentSelectedEntity = temp.entityType;

                    if(this._currentSelectedEntity == EntityType.UNIT) {
                        this._currentStructureSelected = null;
                        this._currentUnitSelected = this._currentSelected as UnitBase;
                    } else if(this._currentSelectedEntity == EntityType.STRUCTURE) {
                        this._currentUnitSelected = null;
                        this._currentStructureSelected = this._currentSelected as StructureBase;
                    }

                    this._previousState = this._currentState;
                    this._currentState = SelectionState.STANDBY;
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
                    this._currentSelected.uiBase.Hide(); // Hide UI;
                    this._currentSelected = null;
                }

                this._currentUnitSelected = null;
                this._currentStructureSelected = null;

                this._previousState = this._currentState;
                this._currentState = SelectionState.FREE;
                this._selected = false;

                if(this._controller.GetType() == typeof(Human))
                    ((Human)this._controller).playerCursor.SetDefault();
            }
        }

        private void StandbyHover() {

            if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.groundLayer)) {

                HasHealthBase temp = this._hitInfo.transform.GetComponent<HasHealthBase>() as HasHealthBase;

                // Currently hovering over a unit or structure.
                if(temp != null) {

                    if(this._currentHover != null) {

                        if(this._currentHover == temp)
                            return;

                        this._previousHover = this._currentHover;
                        this._previousHover.uiBase.OnExit();
                        this._currentHover = temp;
                        this._currentHover.uiBase.OnEnter(this._controller);
                    } else {
                        this._currentHover = temp;
                        this._currentHover.uiBase.OnEnter(this._controller);
                    }

                    // Current Hover is over Enemy.
                    if(this._currentHover.IsEnemy(this._controller)) {

                        if(this._currentSelectedEntity == EntityType.UNIT) {

                            if(this._currentUnitSelected.CanAttack) {
                                if(this._humanController != null)
                                    this._humanController.playerCursor.SetAttackReady();

                                if(this._currentUnitSelected.CanMove) {

                                    //Debug.Log("Move Point : " + this._currentHover.transform.position.ToString());
                                    this._currentUnitSelected.SetTarget(this._currentHover);

                                } else if(this._currentUnitSelected.IsMoving) {

                                    if(this._humanController != null)
                                        this._humanController.playerCursor.SetDefault();

                                    this._currentUnitSelected.unitUI.DisableMovePath();

                                } else {
                                    this._currentUnitSelected.SetTarget(this._currentHover);
                                }

                            } else {
                                if(this._humanController != null)
                                    this._humanController.playerCursor.SetAttackNotReady();
                            }

                        }

                    } else { // Current Hover is over Ally.

                        if(this._currentHover.entityType == EntityType.UNIT) {

                            UnitBase tempUnit = this._currentHover as UnitBase;

                            if(this._humanController != null)
                                this._humanController.playerCursor.SetDefault();

                            // If the CurrentSElected Unit is a Cleric Then change the cursor to healing.
                            if(tempUnit.unitType == UnitType.CLERIC) {

                            }

                        }

                    }
                }

            } else {
                // If im currently just hitting anything other than units or structures e.g. hitting the ground.
                if(this._currentSelectedEntity == EntityType.UNIT) {

                    this._currentUnitSelected.unitUI.DisableAttackRadius();

                    if(this._currentUnitSelected.CanMove) {

                        if(this._humanController != null)
                            this._humanController.playerCursor.SetMoveReady();

                        //Debug.Log("Move Point : " + this.GetCurrentPointOnGround().ToString());
                        this._currentUnitSelected.SetPoint(this.GetCurrentPointOnGround());

                    } else if(this._currentUnitSelected.IsMoving) {
                        if(this._humanController != null)
                            this._humanController.playerCursor.SetDefault();

                        this._currentUnitSelected.unitUI.DisableMovePath();

                    } else {
                        if(this._humanController != null)
                            this._humanController.playerCursor.SetMoveNotReady();

                        this._currentUnitSelected.unitUI.DisableMovePath();
                    }

                } else {

                    if(this._humanController != null)
                        this._humanController.playerCursor.SetDefault();
                }

                if(this._currentHover != null) {
                    this._previousHover = this._currentHover;
                    this._previousHover.uiBase.OnExit();
                }

                this._currentHover = null;
            }
        }

        private void FreeHover() {
            if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.groundLayer)) {

                if(this._hitInfo.transform.GetComponent<HasHealthBase>() != null) {

                    HasHealthBase temp = this._hitInfo.transform.GetComponent<HasHealthBase>() as HasHealthBase;

                    if(this._currentHover != null) {

                        if(this._currentHover == temp)
                            return;

                        this._previousHover = this._currentHover;
                        this._previousHover.uiBase.OnExit();
                        this._currentHover = temp;
                        this._currentHover.uiBase.OnEnter(this._controller);
                    } else {
                        this._currentHover = temp;
                        this._currentHover.uiBase.OnEnter(this._controller);
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