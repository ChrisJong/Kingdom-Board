namespace UI {

    using UnityEngine;

    using Player;

    public abstract class UIBase : MonoBehaviour, IUIComponent {

        #region VARIABLE
        public GameObject goHover;
        public GameObject goSelected;

        private Canvas _canvas;
        private RectTransform _rectTransform;

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
        public virtual void OnEnable() {
            this._canvas = this.GetComponent<Canvas>();
            this._rectTransform = this.GetComponent<RectTransform>();

            if(this.transform.Find("_selected") != null)
                this.goSelected = this.transform.Find("_selected").gameObject;
            if(this.goSelected != null)
                this.goSelected.SetActive(false);

            if(this.transform.Find("_hover") != null)
                this.goHover = this.transform.Find("_hover").gameObject;
            if(this.goHover != null)
                this.goHover.SetActive(false);
        }
        #endregion

        #region CLASS
        public abstract void Display();
        public abstract void Hide();
        protected abstract void Reset();

        public virtual void UpdateUI() {
            Debug.LogWarning("UI update not implemented");
        }
        protected virtual void OnMouseEnter() {
            Debug.Log("Hover");
            if(this.goHover == null)
                return;

            if(this.goSelected.activeSelf || this.goHover.activeSelf)
                return;

            this.goHover.SetActive(true);
        }
        protected virtual void OnMouseExit() {
            if(this.goHover == null)
                return;

            if(this.goSelected.activeSelf || !this.goHover.activeSelf)
                return;

            this.goHover.SetActive(false);
        }
        #endregion
    }
}