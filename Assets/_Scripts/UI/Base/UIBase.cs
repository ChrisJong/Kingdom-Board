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

        [SerializeField] protected GameObject _uiGroup;
        [SerializeField] protected Canvas _uiCanvas;
        [SerializeField] protected Transform _uiGroupTransform;
        [SerializeField] protected RectTransform _uiGroupRectTransform;

        public bool IsActive { get { return this._uiGroup.activeSelf; } set { this._uiGroup.SetActive(value); } }

        public Player Controller { get { return this._controller; } set { this._controller = value; } }

        #endregion

        #region CLASS
        public virtual void Setup() {
            if(this._uiGroup == null) {
                this._uiGroup = this.transform.Find("_UI").gameObject;
                this._uiGroupTransform = this._uiGroup.transform;
                this._uiGroupRectTransform = this._uiGroup.transform as RectTransform;

                this._uiCanvas = this._uiGroup.GetComponent<Canvas>() as Canvas;
            } else {
                Debug.Log("Finding UI Again");
            }
        }

        public virtual void Init(Player controller) {
            this._controller = controller;
        }

        public abstract void DisplayUI();

        public abstract void HideUI();

        public abstract void OnEnter();

        public abstract void OnExit();

        public abstract void ResetUI();

        public abstract void UpdateUI();
        #endregion
    }
}