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

        private Camera _camera = null;

        private Player _controller = null;
        private Human _humanController = null;

        [SerializeField] private SelectionState _previousState = SelectionState.NONE;
        [SerializeField] private SelectionState _currentState = SelectionState.NONE;
        
        [Space]
        [SerializeField] private EntityType _currentSelectedEntity = EntityType.NONE;

        [Space]
        [SerializeField] private HasHealthBase _currentSelected = null;
        [SerializeField] private HasHealthBase _previousSelected = null;
        [SerializeField] private UnitBase _currentUnitSelected = null; // _currentSelectted is type of Unit.
        [SerializeField] private StructureBase _currentStructureSelected = null; // _currentselected is type of structure.

        [Space]
        [SerializeField] private HasHealthBase _currentHover = null;
        [SerializeField] private HasHealthBase _previousHover = null;

        [Space]
        [SerializeField] private HasHealthBase _targetSelected = null;

        [Space]
        private Vector3? _currentHoverPoint = null;
        private Vector3? _pointSelected = null;

        [Header("DEBUGGING")]
        [SerializeField] private Vector3 _debugCurrentPoint = Vector3.zero;

        private Ray _ray;
        private RaycastHit _hitInfo;

        public SelectionState CurrentState { get { return this._currentState; } set { this._previousState = this._currentState; this._currentState = value; } }
        public SelectionState PreviousState { get { return this._previousState; } }

        #endregion

        #region UNITY
        private void Update() {
            if(this._controller.CurrentState != PlayerState.DEAD)
                this.UpdateClass();
        }
        #endregion

        #region PHYSICS_RAYCAST
        private bool GetPointOnGround(out Vector3? point) {
            Vector3 coord = Vector3.zero;
            Ray ray = this._camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            Physics.Raycast(ray, out hitInfo, this._rayDistance, GlobalSettings.LayerValues.groundLayer, QueryTriggerInteraction.Ignore);

            if(Utils.SamplePosition(hitInfo.point, out coord)) {
                Debug.Log(coord.ToString());
                point = coord;
                return true;
            }

            Debug.Log(coord.ToString());
            point = null;
            return false;
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

            return Physics.Raycast(this._ray, out this._hitInfo, this._rayDistance, mask, QueryTriggerInteraction.Ignore);
        }

        private bool CastRayToWorldIgnoreMask(LayerMask ignoreMask) {
            this._ray = this._camera.ScreenPointToRay(Input.mousePosition);

            //Debug.DrawRay(this._ray.origin, this._ray.direction * this._rayDistance, Color.red);

            if(EventSystem.current.IsPointerOverGameObject())
                return false;

            return Physics.Raycast(this._ray, out this._hitInfo, this._rayDistance, ~(ignoreMask), QueryTriggerInteraction.Ignore);
        }

        #endregion

        #region CLASS_CLEAN
        public void Init(Player player) {
            if(this._camera == null) {
                if(this.GetComponent<Camera>() != null) {

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

        public void ChangeState(SelectionState state) {
            if(this._currentState == state)
                return;

            if(state == SelectionState.FREE) {
                if(this._currentSelected != null) {
                    this._currentSelected.uiBase.Hide();
                    this._previousSelected = this._currentSelected;
                }

                this._currentSelected = null;
            }

            this._previousState = this._currentState;
            this._currentState = state;
        }

        private void UpdateClass() {
            Debug.DrawRay(this._ray.origin, this._ray.direction * this._rayDistance, Color.yellow);
            this.MouseSelection();
        }

        private void MouseSelection() {
            switch(this._currentState) {
                case SelectionState.FREE:
                this.FreeState();
                break;

                case SelectionState.STANDBY:
                this.StandbyState();
                break;

                case SelectionState.SPAWNPOINT:
                this.SpawnPointState();
                break;

                case SelectionState.WAITING:
                this.WaitingState();
                break;
            }
        }

        private void Selection() {
            HasHealthBase temp = this._hitInfo.transform.GetComponent<HasHealthBase>();

            if(temp != null) {

                if(this._controller.CurrentState == PlayerState.DEFENDING && temp.entityType == EntityType.UNIT)
                    return;

                if(temp.IsEnemy(this._controller))
                    return;

                if(temp.entityType == EntityType.UNIT) {
                    if(((UnitBase)temp).CurrentState == UnitState.FINISH)
                        return;
                }

                if(temp.Controller.id == this._controller.id) {

                    if(this._currentSelected != null && !temp.Equals(this._currentSelected)) {
                        this._previousSelected = this._currentSelected;
                        this._previousSelected.uiBase.Hide();
                    }

                    this._currentSelected = temp;
                    this._currentSelected.uiBase.Display();
                    this._currentSelectedEntity = temp.entityType;

                    if(this._currentSelectedEntity == EntityType.UNIT) { // Selected a Unit
                        this._currentStructureSelected = null;
                        this._currentUnitSelected = this._currentSelected as UnitBase;
                    } else if(this._currentSelectedEntity == EntityType.STRUCTURE) { // SElected a Structure
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
            }
        }
        #endregion

        #region CLASS_STATE
        private void FreeState() {
            // Left Click!
            if(Input.GetMouseButtonUp(0)) {
                if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.groundLayer))
                    this.Selection();
            } else
                this.FreeStateHover();
        }

        private void FreeStateHover() {
            if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.groundLayer)) {

                HasHealthBase temp = this._hitInfo.transform.GetComponent<HasHealthBase>();

                if(temp != null) {

                    if(temp.IsAlly(this._controller))
                        if(this._humanController != null)
                            this._humanController.playerCursor.ChangeState(CursorState.SELECTED);

                    if(this._currentHover != null && this._currentHover != temp) {
                        this._currentHover.uiBase.OnExit();
                        this._previousHover = this._currentHover;
                    }

                    this._currentHover = temp;
                    this._currentHover.uiBase.OnEnter(this._controller);
                }

            } else {

                if(this._humanController != null)
                    this._humanController.playerCursor.ChangeState(CursorState.DEFAULT);

                if(this._currentHover != null) {
                    this._currentHover.uiBase.OnExit();
                    this._previousHover = this._currentHover;
                }

                this._currentHover = null;
            }
        }

        private void StandbyState() {

            // Right Click!
            if(Input.GetMouseButtonUp(1)) {

                // Hit Unit/Structure.
                if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.groundLayer)) {

                    this._targetSelected = this._hitInfo.transform.GetComponent<HasHealthBase>();

                } else { // we hit empty space.

                    if(this._currentSelectedEntity == EntityType.UNIT) {
                        if(this.GetPointOnGround(out this._pointSelected)) {

                            if(this._currentUnitSelected.SetPoint(this._pointSelected.Value)) {
                                this._currentUnitSelected.InitiateMove();

                                this.ChangeState(SelectionState.WAITING);
                                this._debugCurrentPoint = this._pointSelected.Value;
                            }
                        }
                    }
                }

            } else if(Input.GetMouseButtonUp(0)) { // Left Click!

                if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.groundLayer))
                    this.Selection();
                else
                    this.Deselection();

            } else
                this.StandbyStateHover(); // Hovering over stuff.
        }

        private void StandbyStateHover() {
            // Selected anything other than the ground e.g. units/structures.
            if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.groundLayer)) {

                HasHealthBase temp = this._hitInfo.transform.GetComponent<HasHealthBase>();

                // Hovering Over a Unit/STructure.
                if(temp != null) {

                    if(this._currentHover != null && this._currentHover != temp) {
                        this._currentHover.uiBase.OnExit();
                        this._previousHover = this._currentHover;
                    }

                    this._currentHover = temp;
                    this._currentHover.uiBase.OnEnter(this._controller);

                    if(this._currentSelected.IsEnemy(temp)) { // Current Hover is an Enemy.

                        if(this._currentSelectedEntity == EntityType.UNIT) {

                            if(this._currentUnitSelected.CanAttack) {

                                if(this._currentUnitSelected.SetTarget(temp)) {
                                    if(this._humanController != null)
                                        this._humanController.playerCursor.ChangeState(CursorState.ATTACK);
                                } else {
                                    if(this._humanController != null)
                                        this._humanController.playerCursor.ChangeState(CursorState.ATTACK_NOTREADY);
                                }

                            } else {
                                if(this._humanController != null)
                                    this._humanController.playerCursor.ChangeState(CursorState.ATTACK_NOTREADY);
                            }
                        }

                    } else { // Current Hover is an Ally.

                        if(this._currentSelectedEntity == EntityType.UNIT) {

                            if(this._currentUnitSelected.unitType == UnitType.CLERIC) {

                                Cleric cleric = this._currentUnitSelected as Cleric;

                                if(cleric.CanHeal) {

                                    if(cleric.SetTarget(temp)) {
                                        if(this._humanController != null)
                                            this._humanController.playerCursor.ChangeState(CursorState.HEAL);
                                    } else {
                                        if(this._humanController != null)
                                            this._humanController.playerCursor.ChangeState(CursorState.HEAL_NOTREADY);
                                    }

                                } else {
                                    if(this._humanController != null)
                                        this._humanController.playerCursor.ChangeState(CursorState.HEAL_NOTREADY);
                                }
                            } else {
                                if(temp.IsAlly(this._controller))
                                    if(this._humanController != null)
                                        this._humanController.playerCursor.ChangeState(CursorState.SELECTED);
                            }
                        }
                    }
                }

            } else { // Hit Empty Space (Eg. the ground)

                // Set the Move target for the selected unit if possible.
                if(this._currentSelectedEntity == EntityType.UNIT) {

                    if(this._currentUnitSelected.CanMove) {

                        if(this.GetPointOnGround(out this._currentHoverPoint)) {
                            if(this._humanController != null)
                                this._humanController.playerCursor.ChangeState(CursorState.MOVE);

                            this._debugCurrentPoint = this._currentHoverPoint.Value;
                            this._currentUnitSelected.SetPoint(this._currentHoverPoint.Value);

                        } else {
                            if(this._humanController != null)
                                this._humanController.playerCursor.ChangeState(CursorState.MOVE_NOTREADY);

                            this._debugCurrentPoint = Vector3.zero;
                        }

                    } else {
                        if(this._humanController != null)
                            this._humanController.playerCursor.ChangeState(CursorState.MOVE_NOTREADY);
                    }

                } else {
                    if(this._humanController != null)
                        this._humanController.playerCursor.ChangeState(CursorState.DEFAULT);
                }

                if(this._currentHover != null) {
                    this._currentHover.uiBase.OnExit();
                    this._previousHover = this._currentHover;
                }

                this._currentHover = null;
            }
        }

        // NOTE: Doesn't Check if units are already at the specified location.
        private void SpawnPointState() {
            // Left Click!
            if(Input.GetMouseButtonUp(0)) {

                if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.unitLayer)) {

                    if(this._controller.castle != null) {

                        if(!this._controller.castle.SetPoint(this._hitInfo.point)) {
                            Debug.LogWarning("The Location Selected Is Invalud: " + this._hitInfo.point.ToString());
                            return;
                        } else {
                            this._currentState = this._previousState;
                            this._previousState = SelectionState.NONE;
                        }

                    } else
                        Debug.LogError("Player_" + this._controller.id.ToString() + "Doesn't Contain A Castle Object");
                }

            } else if(Input.GetMouseButtonUp(1)) {

                if(this._controller.castle != null)
                    this._controller.castle.CancelSpawn();
                else
                    Debug.LogError("Player_" + this._controller.id.ToString() + "Doesn't Contain A Castle Object");

                this._currentState = this._previousState;
                this._previousState = SelectionState.NONE;

            } else {

                if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.unitLayer))
                    this._controller.castle.CheckSpawnPoint(this._hitInfo.point);
                else
                    this._controller.castle.HidePlavement();

            }
        }

        private void WaitingState() {

        }
        #endregion

        // REMOVE
        /*private void StandbyState2() {

            // Right Click.
            if(Input.GetMouseButtonUp(1)) {

                // Cast A Ray Ignoring the ground layer.
                if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.groundLayer)) {

                    HasHealthBase temp = this._hitInfo.transform.GetComponent<HasHealthBase>() as HasHealthBase;

                    // Hit Either a unit or structure
                    if(temp != null) {

                        if(this._currentSelectedEntity == EntityType.UNIT) {

                            if(temp.IsEnemy(this._currentUnitSelected)) { // Enemy Unit
                                this._currentUnitSelected.InitiateTarget();
                            } else { // Ally Unit

                                if(this._currentUnitSelected.unitType == UnitType.CLERIC)
                                    this._currentUnitSelected.InitiateTarget();
                            }

                        } else if(this._currentSelectedEntity == EntityType.STRUCTURE) {

                        } else {
                            Debug.LogError("No Such Entity Type For the Current Selection Exsists, " + this._currentSelected.entityType.ToString());
                            throw new System.ArgumentNullException("No Such Entity Type For the Current Selection Exsists, " + this._currentSelected.entityType.ToString());
                        }

                    }

                } else {
                    // Hit nothing but air, soo check for the ground.
                    if(this._currentSelectedEntity == EntityType.UNIT) {

                        if(this.CastRayToWorldToMask(GlobalSettings.LayerValues.groundLayer))
                            this._currentUnitSelected.InitiateMove();

                    }
                }

            } else if(Input.GetMouseButtonUp(0)) { // Left Click. (De-Selection)

                if(this.CastRayToWorldIgnoreMask(GlobalSettings.LayerValues.groundLayer))
                    this.Selection();
                else
                    this.Deselection();

            } else {
                this.StandbyHover();
            }
        }*/

    }
}