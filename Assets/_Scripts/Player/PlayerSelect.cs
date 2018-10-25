namespace Player {

    using System;

    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.EventSystems;

    using Constants;
    using Enum;
    using Helpers;
    using Structure;
    using Unit;
    using Utility;

    public class PlayerSelect : MonoBehaviour {
        #region VARIABLE
        private Camera _camera;
        private Player _controller;

        // SELECTION
        public HasHealthBase currentSelected;
        public HasHealthBase previousSelected;

        public Vector3 moveTo;
        public HasHealthBase targetSelected;

        //public Selectable currentHover;
        //public Selectable previousHover;

        public bool selected = false;
        public bool lockSelection = false;

        // PHYSICS (RAY)
        private float _distance = 50.0f;

        private Ray _ray;
        private RaycastHit _hitInfo;
        #endregion

        #region UNITY
        private void Awake() {
            Camera playerCamera = this.GetComponent<Camera>() as Camera;
            if(playerCamera == null)
                Debug.LogError("Player Controller Needs A CAmera script");

            if(this._camera == null)
                this._camera = this.GetComponent<Camera>() as Camera;
        }

        private void Update() {
            this.UpdatePlayerSelect();
        }
        #endregion

        #region CLASS
        public void Init(Player player) {
            this._controller = player;
        }

        public void EndTurn() {
            if(this.currentSelected != null && this.currentSelected.uiComponent.showSelected)
                this.currentSelected.uiComponent.Hide();

            this.currentSelected = null;
            this.previousSelected = null;
        }

        public Vector3 GetCurrentPointOnGround() {
            Vector3 coord = Vector3.zero;
            Ray ray = this._camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            Physics.Raycast(ray, out hitInfo, this._distance, GlobalSettings.LayerValues.groundLayer);

            coord = hitInfo.point;

            return coord;
        }

        private void UpdatePlayerSelect() {
            Debug.DrawRay(this._ray.origin, this._ray.direction * this._distance, Color.yellow);
            this.MouseSelection();
        }

        private bool CastRayToWorld(LayerMask mask) {
            this._ray = this._camera.ScreenPointToRay(Input.mousePosition);

            //Debug.DrawRay(this._ray.origin, this._ray.direction * this._distance, Color.yellow);

            if(EventSystem.current.IsPointerOverGameObject())
                return false;

            return Physics.Raycast(this._ray, out this._hitInfo, this._distance, ~(mask));
        }

        private void MouseSelection() {
            switch(this._controller.selectionState) {
                case SelectionState.FREE:
                if(Input.GetMouseButtonUp(0)) {
                    if(this.CastRayToWorld(GlobalSettings.LayerValues.groundLayer)) {
                        this.FreeSelection();
                    } else {
                        this.FreeDeselection();
                    }
                }
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

        private void FreeSelection() {
            if(this._hitInfo.transform.GetComponent<HasHealthBase>() != null) {
                var temp = this._hitInfo.transform.GetComponent<HasHealthBase>();

                if(temp.entityType == EntityType.UNIT) {
                    if(((UnitBase)temp).unitState == UnitState.FINISH)
                        return;
                }

                if(temp.controller.id == this._controller.id) {
                    if(this.currentSelected != null && !temp.Equals(this.currentSelected)) {
                        this.previousSelected = this.currentSelected;
                        this.previousSelected.uiComponent.Hide();
                    }

                    this.currentSelected = temp;
                    this.currentSelected.uiComponent.Display();

                    this.selected = true;
                }
            } else {
                Debug.LogWarning(this._hitInfo.transform.name + " Doesn't Derive From HasHealthBase Class.");
            }
        }

        private void FreeDeselection() {
            if(!EventSystem.current.IsPointerOverGameObject()) {
                if(this.currentSelected != null) {
                    this.previousSelected = this.currentSelected;
                    this.currentSelected.uiComponent.Hide(); // Hide UI;
                    this.currentSelected = null;
                }
                this.selected = false;
            }
        }

        private void SelectPoint() {
            if(Input.GetMouseButtonUp(0)) {
                if(this.CastRayToWorld(GlobalSettings.LayerValues.unitLayer)) {

                    if(this.currentSelected is ISelected) {
                        if(!(this.currentSelected as ISelected).SetPoint(this._hitInfo.point)) {
                            Debug.LogWarning("The Location Selected Is invalid: " + this._hitInfo.point.ToString());
                            return;
                        }
                    } else {
                        Debug.LogWarning(this.currentSelected.name + " Doesn't Inherit from ISelection Interface.");
                    }
                }
            }
        }

        private void SelectTarget() {
            if(Input.GetMouseButtonUp(0)) {
                if(this.CastRayToWorld(GlobalSettings.LayerValues.groundLayer)) {
                    if(this._hitInfo.transform.GetComponent<HasHealthBase>() != null) {
                        targetSelected = this._hitInfo.transform.GetComponent<HasHealthBase>();

                        if(this.CheckTargetSelection(targetSelected)) {
                            if(this.currentSelected is ISelected) {
                                if(!(this.currentSelected as ISelected).SetTarget(targetSelected as IHasHealth)) {
                                    Debug.Log("Unit out of range");
                                    return;
                                }
                            } else {
                                Debug.LogWarning(this.currentSelected.name + " Deson't Inherit from ISelection Interface.");
                            }
                        } else {
                            Debug.LogWarning(targetSelected.name + " - Entity Isn't Apart Of Any Player Group.");
                            return;
                        }
                    }
                }
            }
        }

        // NOTE: this needs to be changed let the unit/structure figure out if the targetselected needs to be an ally or an enemy.
        private bool CheckTargetSelection(HasHealthBase target) {
            if(this._controller.selectionState == SelectionState.SELECT_ENEMYTARGET) {
                return this.currentSelected.IsEnemy(target);
            } else if(this._controller.selectionState == SelectionState.SELECT_ALLYTARGET) {
                return this.currentSelected.IsAlly(target);
            } else if(this._controller.selectionState == SelectionState.SELECT_TARGET) {
                return true;
            } else {
                return false;
            }
        }

        private void SelectSpawnPoint() {

        }

        private void DebugText(UnitBase unit, UnitBase selection) {
            float damage = unit.GetDamage();
            float extraDamage = 0.0f;
            string damagetext = damage.ToString() + "(" + "+" +extraDamage.ToString() + ")";

            if(unit.attackType == selection.weaknessType) {
                extraDamage = (damage * (selection.weaknessPercentage / 100.0f));
                damagetext = damage.ToString() + "(" + "+" + extraDamage.ToString() + ")";
            }
            if(unit.attackType == selection.resistanceType) {
                extraDamage = damage * (selection.resistancePercentage / 100.0f);
                damagetext = damage.ToString() + "(" + "-" + extraDamage.ToString() + ")";
            }

            string temp = "\r\n" + unit.name + " Attacking: " + selection.name + " For " + damagetext + " DAMAGe";

            this._controller.uiComponent.ChangeDebugText(temp);
        }

        // NOTE: instead of using this i've used the unity OnMouseEnter/Exit to simulate hovering over an object.
        /*private void HoverSelect() {
            if(this.CastRayToWorld()) {
                if(this._hitInfo.transform.GetComponent<Selectable>() != null) {
                    if(this.currentHover != null) {
                        this.previousHover = this.currentHover;
                        this.currentHover = this._hitInfo.transform.GetComponent<Selectable>() as Selectable;
                    } else {
                        this.currentHover = this._hitInfo.transform.GetComponent<Selectable>() as Selectable;
                    }
                }
            }else {
                if(this.currentHover != null)
                    this.previousHover = this.currentHover;
                this.currentHover = null;
            }
        }*/
        #endregion
    }
}