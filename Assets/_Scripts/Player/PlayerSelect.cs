namespace Player {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.EventSystems;

    using Constants;
    using Helpers;
    using Unit;
    using Structure;

    public class PlayerSelect : MonoBehaviour {
        #region VARIABLE
        private int _playerID;

        private Camera _camera;
        private Player _player;

        // SELECTION
        public IHasHealth currentSelected;
        public IHasHealth previousSelected;

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
            if(player == null)
                Debug.LogError("Player Missing!");

            this._player = player;
        }

        private void UpdateSelect() {
            this.CastRayToWorld();
            this.MouseSelect();
            //this.HoverSelect();
        }

        private bool CastRayToWorld() {
            this._ray = this._camera.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(this._ray.origin, this._ray.direction * this._distance, Color.yellow);

            return Physics.Raycast(this._ray, out this._hitInfo, this._distance, ~(GlobalSettings.LayerValues.groundLayer));
        }

        private void MouseSelect() {
            if(Input.GetMouseButtonUp(0)) {

                if(this.CastRayToWorld()) {
                    //Selectable tempSelect = this._hitInfo.transform.GetComponent<Selectable>() as Selectable;
                    if(!this._player.turnEnded)
                        this.SelectObject();
                } else {
                    if(!this._player.turnEnded)
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
            if(this._hitInfo.transform.GetComponent<IHasHealth>() != null) {
                this.currentSelected = this._hitInfo.transform.GetComponent<IHasHealth>();

                // Show UI;
                this.currentSelected.uiComponent.DisplayUI();

                this.selected = true;
            }
        }

        private void DeSelectObject() {
            if(!EventSystem.current.IsPointerOverGameObject()) {
                if(this.currentSelected != null) {
                    this.previousSelected = this.currentSelected;

                    // Hide UI;
                    this.currentSelected.uiComponent.HideUI();

                    this.currentSelected = null;
                }

                this.selected = false;
            }
        }
        #endregion
    }
}