namespace UI {

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    using Constants;
    using Player;

    // NOTES: https://stackoverflow.com/questions/41391708/how-to-detect-click-touch-events-on-ui-and-gameobjects
    // NOTES: https://answers.unity.com/questions/1077069/implementing-ipointerclickhandler-interface-does-n.html
    // UI needs input needs to changed to use EventSystems For Mouse & Touch Inputs.

    [System.Serializable]
    public abstract class UIBase : MonoBehaviour, IUIComponent {

        #region VARIABLE

        [SerializeField] protected Player _controller;

        [Space]
        [SerializeField] protected GameObject _mainGroup;
        [SerializeField] protected Canvas _mainCanvas;
        [SerializeField] protected RectTransform _mainRectTransform;

        public bool IsActive { get { return this._mainGroup.activeSelf; } set { this._mainGroup.SetActive(value); } }

        public Player Controller { get { return this._controller; } set { this._controller = value; } }

        #endregion

        #region CLASS
        public virtual void Setup() {
            this._mainGroup = this.transform.Find(UIValues.MAIN_GROUP).gameObject;
            if(this._mainGroup != null) {
                this._mainGroup = this.transform.Find(UIValues.MAIN_GROUP).gameObject;
                this._mainRectTransform = this._mainGroup.transform as RectTransform;
                this._mainCanvas = this._mainGroup.GetComponent<Canvas>() as Canvas;
            }
        }

        public virtual void Init(Player controller) {
            this._controller = controller;
        }

        public virtual void Return() {
            this._controller = null;

            this._mainCanvas = null;
            this._mainRectTransform = null;
            this._mainGroup = null;
        }

        public abstract void UpdateUI();

        public abstract void Display();

        public abstract void Hide();

        public abstract void OnEnter();

        public abstract void OnEnter(Player controller);

        public abstract void OnExit();

        public abstract void ResetUI();
        #endregion
    }
}