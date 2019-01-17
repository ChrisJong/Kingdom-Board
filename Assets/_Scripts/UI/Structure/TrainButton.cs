namespace UI {

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using TMPro;

    using Enum;
    using Scriptable;
    using Structure;

    public class TrainButton : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {

        #region VARIABLE

        [SerializeField] private Castle _castle;

        [SerializeField] private UnitClassType _classType = UnitClassType.NONE;
        [SerializeField] private UnitType _unitType = UnitType.NONE;

        [SerializeField] private bool _isLocked = true;
        [SerializeField] private bool _isInfoDisplayed = false;

        [SerializeField] private Sprite _unitIconSprite;

        private RectTransform _rectTransform;
        private Button _button;

        private GameObject _gameObject;
        [SerializeField] private GameObject _unitIconObject;
        [SerializeField] private GameObject _lockedObject;
        [SerializeField] private GameObject _infoObject;

        private Image _image;
        [SerializeField] private Image _unitIconImage;
        [SerializeField] private Image _lockedmage;

        [SerializeField] private TextMeshProUGUI _infoText;

        public UnitClassType ClassType { get { return this._classType; } }

        public UnitType UnitType { get { return this._unitType; } }

        public bool IsLocked { get { return this._isLocked; } }

        #endregion

        #region UNITY

        public void OnPointerUp(PointerEventData eventData) {
            if(this._isLocked)
                return;

            this.AddToQueue();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if(!this._isLocked && !this._isInfoDisplayed) {
                this._infoObject.SetActive(true);
                this._isInfoDisplayed = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            if(!this.IsLocked && this._isInfoDisplayed) {
                this._infoObject.SetActive(false);
                this._isInfoDisplayed = false;
            }
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

            this._unitIconSprite = data.iconSprite;
            this._unitIconImage.sprite = this._unitIconSprite;

            this._infoText.text = "G: " + data.goldCost.ToString() + "\r\n" +
                                  "P: " + data.populationCost.ToString();
            this._infoObject.SetActive(false);

            this._isLocked = locked;
            this._isInfoDisplayed = false;

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
