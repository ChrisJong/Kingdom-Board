namespace UI {

    using System;

    using UnityEngine;
    using UnityEngine.UI;

    using Constants;
    using Player;

    public abstract class UIBase : MonoBehaviour, IUIComponent {

        #region VARIABLE
        public Transform tUI;
        protected Transform _tHover;
        protected Transform _tSelected;

        public bool showSelected = false;
        public bool showHover = false;
        protected GameObject _goUI;
        protected GameObject _goHover;
        protected GameObject _goSelected;

        protected Canvas _canvas;
        protected RectTransform _rectTransform;

        private Player _controller;

        public Canvas canvas { get { return this._canvas; } }
        public RectTransform rectTransform { get { return this._rectTransform; } }

        public Player controller {
            get { return this._controller; }
            set { this._controller = value; } }

        public bool isActive {
            get { return this.gameObject.activeSelf; }
            set { this.gameObject.SetActive(value); } }
        #endregion

        #region UNITY
        protected virtual void Awake() {
            if(this.tUI == null) {
                Debug.LogError("PLEASE CONNECT THE UI GAMEOBJECT BEFORE STARTING.");
                throw new ArgumentNullException("UI GAMEOBJECT NOT FOUND");
            }

            this._canvas = this.tUI.GetComponent<Canvas>();
            this._rectTransform = this.tUI.GetComponent<RectTransform>();

            if(this.tUI.Find(UIValues.SELECTEDGROUP) != null && this._tSelected == null) {
                this._tSelected = this.tUI.Find(UIValues.SELECTEDGROUP);
                this._goSelected = this._tSelected.gameObject;
                this._goSelected.SetActive(false);
            }else if(this._tSelected != null) {
                this._goSelected = this._tSelected.gameObject;
                this._goSelected.SetActive(false);
            }

            if(this.tUI.Find(UIValues.HOVERGROUP) != null && this._tHover == null) {
                this._tHover = this.tUI.Find(UIValues.HOVERGROUP);
                this._goHover = this._tHover.gameObject;
                this._goHover.SetActive(false);
            }else if(this._tHover != null) {
                this._goHover = this._tHover.gameObject;
                this._goHover.SetActive(false);
            }
        }

        protected virtual void OnMouseEnter() {
            if(this._tHover == null)
                return;

            if(this._goSelected.activeSelf || this._goHover.activeSelf)
                return;

            this._goHover.SetActive(true);
            this.showHover = true;
        }

        protected virtual void OnMouseExit() {
            if(this._tHover == null)
                return;

            if(this._goSelected.activeSelf || !this._goHover.activeSelf)
                return;

            this._goHover.SetActive(false);
            this.showHover = false;
        }
        #endregion

        #region CLASS
        public virtual void Display() {
            this.showSelected = true;
        }
        public virtual void Hide() {
            this.showSelected = false;
        }
        protected abstract void ResetUI();

        public virtual void UpdateUI() {
            Debug.LogWarning("UI update not implemented");
        }

        protected virtual void FindUI(Transform parent, string name) {
            if(this.tUI != null)
                return;

            if(parent.Find(name) != null) {
                this.tUI = parent.Find(name);
                this._goUI = this.tUI.gameObject;
            } else {
                Debug.LogError("NO PLAYER UI FOUND");
                throw new ArgumentNullException("NO PLAYER UI FOUND");
            }
        }

        protected virtual Button FindButton(Transform parent, string name) {
            Button btn = null;

            if(parent.Find(name) != null)
                btn = parent.Find(name).GetComponent<Button>() as Button;
            else
                Debug.LogError("BUTTON (" + name + ") not found within " + parent.name + " object");

            if(btn == null)
                throw new ArgumentNullException("Null: Button not found or doesn't exist");

            return btn;
        }
        #endregion
    }
}