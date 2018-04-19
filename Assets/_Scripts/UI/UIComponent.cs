namespace UI {

    using UnityEngine;
    using UnityEngine.UI;

    using Player;

    public abstract class UIComponent : MonoBehaviour, IUIComponent {

        #region VARIABLE
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

        #region cLASS
        public abstract void DisplayUI();
        public abstract void HideUI();
        public virtual void UpdateUI() {
            Debug.LogWarning("Update UI Function Not Implemented");
        }
        protected abstract void ResetUI();
        #endregion
    }
}