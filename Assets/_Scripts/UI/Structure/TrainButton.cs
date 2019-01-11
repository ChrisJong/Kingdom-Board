namespace UI {

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    using Enum;
    using Scriptable;
    using Structure;

    public class TrainButton : MonoBehaviour, IPointerUpHandler {

        #region VARIABLE

        [SerializeField] private Castle _castle;

        [SerializeField] private ClassType _classType = ClassType.NONE;
        [SerializeField] private UnitType _unitType = UnitType.NONE;

        [SerializeField] private bool _isLocked = true;

        [SerializeField] private Sprite _unitIconSprite;

        private RectTransform _rectTransform;
        private Button _button;

        private GameObject _gameObject;
        [SerializeField] private GameObject _unitIconObject;
        [SerializeField] private GameObject _lockedObject;

        private Image _image;
        [SerializeField] private Image _unitIconImage;
        [SerializeField] private Image _lockedmage;

        public ClassType ClassType { get { return this._classType; } }

        public UnitType UnitType { get { return this._unitType; } }

        public bool IsLocked { get { return this._isLocked; } }

        #endregion

        #region UNITY

        public void OnPointerUp(PointerEventData eventData) {
            if(this._isLocked)
                return;

            this.AddToQueue();
        }

        #endregion

        #region CLASS

        public void Init(Castle castle, UnitScriptable data, bool locked = true) {

            this._castle = castle;

            this._rectTransform = this.transform as RectTransform;
            this._gameObject = this.gameObject as GameObject;
            this._button = this.transform.GetComponent<Button>() as Button;
            this._image = this.transform.GetComponent<Image>() as Image;

            this._classType = data.classType;
            this._unitType = data.unitType;

            this._unitIconSprite = data.TrainIconSprite;
            this._unitIconImage.sprite = this._unitIconSprite;

            this._isLocked = locked;

            if(locked)
                this._lockedObject.SetActive(true);
            else
                this._lockedObject.SetActive(false);

        }

        public void Lock() {
            this._isLocked = true;
            this._lockedObject.SetActive(true);
        }

        public void Unlock() {
            this._isLocked = false;
            this._lockedObject.SetActive(false);
        }

        private void AddToQueue() {
            this._castle.AddUnitToQueue(this._classType, this._unitType);
        }

        #endregion
    }
}
