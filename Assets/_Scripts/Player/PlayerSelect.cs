namespace Player {

    using UnityEngine;
    using UnityEngine.EventSystems;

    using Constants;
    using Enum;
    using Helpers;
    using Unit;

    public class PlayerSelect : MonoBehaviour {
        #region VARIABLE
        private Camera _camera;
        private Player _controller;

        // SELECTION
        public HasHealthBase currentSelected;
        public HasHealthBase previousSelected;

        public Vector3 moveTo;
        public HasHealthBase toAttack;

        //public Selectable currentHover;
        //public Selectable previousHover;

        public bool selected = false;
        public bool selectedLock = false;

        // PHYSICS (RAY)
        private float _distance = 50.0f;

        private Ray _ray;
        private RaycastHit _hitInfo;
        #endregion

        #region UNITY
        private void Awake() {
            this._camera = this.GetComponent<Camera>() as Camera; 
        }

        private void Update() {
            this.UpdateSelect();
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

        private void UpdateSelect() {
            switch(this._controller.selectionState) {
                case SelectionState.FREE:
                    this.CastRayToWorld(GlobalSettings.LayerValues.groundLayer);
                    this.MouseSelect();
                    //this.HoverSelect();
                break;

                case SelectionState.RESEARCH:
                    Debug.Log("IN Research Mode");
                break;

                case SelectionState.UNIT_ATTACK:
                    this.AttackSelection();
                break;

                case SelectionState.UNIT_MOVE:
                    this.MoveToSelection();
                break;

                default:
                    this.CastRayToWorld(GlobalSettings.LayerValues.groundLayer);
                    this.MouseSelect();
                    //this.HoverSelect();
                break;
            }
        }

        private bool CastRayToWorld(LayerMask ignoreMask) {
            this._ray = this._camera.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(this._ray.origin, this._ray.direction * this._distance, Color.yellow);

            return Physics.Raycast(this._ray, out this._hitInfo, this._distance, ~(ignoreMask));
        }

        private void MouseSelect() {
            if(Input.GetMouseButtonUp(0)) {

                if(this.CastRayToWorld(GlobalSettings.LayerValues.groundLayer)) {
                    //Selectable tempSelect = this._hitInfo.transform.GetComponent<Selectable>() as Selectable;
                        this.SelectObject();
                } else {
                        this.DeSelectObject();
                }
            }
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

        private void SelectObject() {
            if(this._hitInfo.transform.GetComponent<HasHealthBase>() != null) {
                var temp = this._hitInfo.transform.GetComponent<HasHealthBase>();

                if(temp.entityType == Enum.EntityType.UNIT) {
                    if(((UnitBase)temp).hasFinished)
                        return;
                }

                if(temp.controller.id == this._controller.id) {
                    if(this.currentSelected != null && !temp.Equals(this.currentSelected)) {
                        this.previousSelected = this.currentSelected;
                        this.previousSelected.uiComponent.Hide();
                    }

                    // get current selected
                    this.currentSelected = temp;

                    // display ui
                    this.currentSelected.uiComponent.Display();

                    this.selected = true;
                }
            }
        }

        private void DeSelectObject() {
            if(!EventSystem.current.IsPointerOverGameObject()) {
                if(this.currentSelected != null) {
                    this.previousSelected = this.currentSelected;
                    this.currentSelected.uiComponent.Hide(); // Hide UI;
                    this.currentSelected = null;
                }
                this.selected = false;
            }
        }

        private void AttackSelection() {
            if(Input.GetMouseButtonUp(0)) {
                if(this.CastRayToWorld(GlobalSettings.LayerValues.groundLayer)) {
                    Debug.Log("Selection Distance: " + Vector3.Distance(this.currentSelected.position, this._hitInfo.point));
                    Debug.Log("Unit Radius: " + ((UnitBase)this.currentSelected).unitRadius);
                    if(Vector3.Distance(this.currentSelected.position, this._hitInfo.point) + ((UnitBase)this.currentSelected).unitRadius > ((UnitBase)this.currentSelected).attackRadius) {

                        Debug.Log("Out Of Unit Attack Radius");
                        return;
                    } else {

                    }
                }
            }
        }

        private void MoveToSelection() {
            if(Input.GetMouseButtonUp(0)) {
                if(this.CastRayToWorld(GlobalSettings.LayerValues.unitLayer)) {
                    Debug.Log("Selection Distance: " + Vector3.Distance(this.currentSelected.position, this._hitInfo.point));
                    Debug.Log("Unit Radius: " + ((UnitBase)this.currentSelected).unitRadius);
                    if(Vector3.Distance(this.currentSelected.position, this._hitInfo.point) + ((UnitBase)this.currentSelected).unitRadius > ((UnitBase)this.currentSelected).moveRadius) {
                        
                        Debug.Log("Out Of Unit Move Radius");
                        return;
                    }else {
                        ((UnitBase)this.currentSelected).MoveTo(this._hitInfo.point);
                    }
                }
            }
        }
        #endregion
    }
}