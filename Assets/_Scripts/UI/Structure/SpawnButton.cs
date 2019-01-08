namespace UI {

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    using Enum;
    using Structure;

    public class SpawnButton : MonoBehaviour, IPointerUpHandler {

        #region VARIABLE

        [SerializeField] private Castle _Castle;

        [SerializeField] private ClassType _classType = ClassType.NONE;
        [SerializeField] private UnitType _unitType = UnitType.NONE;

        [SerializeField] private bool _isLocked = true;

        [SerializeField] private Sprite _unitIconSprite;
        [SerializeField] private Sprite _lockedSprite;
        [SerializeField] private Sprite _backgroundSprite;

        private RectTransform _rectTransform;
        private Button _button;

        private GameObject _gameObject;
        private GameObject _unitIconObject;
        private GameObject _lockedObject;

        private Image _image;
        private Image _unitIconImage;
        private Image _lockedmage;

        public ClassType ClassType {
            get { return this._classType; }
        }

        public UnitType UnitType {
            get { return this._unitType; }
        }

        public bool IsLocked {
            get { return this._isLocked; }
        }

        #endregion

        #region UNITY

        public void OnPointerUp(PointerEventData eventData) {
            if(!this._isLocked)
                this._Castle.AddToQueue(this._unitType, this._unitIconSprite);
            else
                return;
        }

        #endregion

        #region CLASS

        public void Init(Castle castle, Sprite unitIconSprite, Sprite lockedSprite, ClassType classType = ClassType.NONE, UnitType unitType = UnitType.NONE, bool locked = true) {

            this._Castle = castle; 

            this._rectTransform = this.transform as RectTransform;
            this._gameObject = this.gameObject as GameObject;
            this._button = this.transform.GetComponent<Button>() as Button;
            this._image = this.transform.GetComponent<Image>() as Image;

            this._classType = classType;
            this._unitType = unitType;

            this._unitIconSprite = unitIconSprite;
            this._lockedSprite = lockedSprite;

            this._isLocked = locked;

            if(locked) {

                if(this._lockedSprite != null)
                    this._image.sprite = this._lockedSprite;

            } else {

                if(this._unitIconSprite != null)
                    this._image.sprite = this._unitIconSprite;

            }
        } 

        public void Lock() {
            this._isLocked = true;
            this._image.sprite = this._lockedSprite;
        }

        public void Unlock() {
            this._isLocked = false;
            this._image.sprite = this._unitIconSprite;
        }

        #endregion
    }
}
