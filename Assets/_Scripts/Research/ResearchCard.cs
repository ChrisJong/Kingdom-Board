namespace KingdomBoard.Research {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using TMPro;

    using Enum;

    [RequireComponent(typeof(ResearchCardAnimation))]
    public abstract class ResearchCard : MonoBehaviour, IPointerUpHandler {

        #region VARIABLE

        [SerializeField] protected Research _research;
        [SerializeField] protected ResearchCardAnimation _cardAnimation;

        [SerializeField] protected UnitClassType _classType = UnitClassType.NONE;
        [SerializeField] protected UnitType _unitType = UnitType.NONE;
        [SerializeField] protected UnitUpgradeType _upgradeType = UnitUpgradeType.NONE;

        [SerializeField] protected int _keyID = -1; // -1 Doesn't belong in anything.

        [SerializeField] protected float _width = 0.0f;
        [SerializeField] protected float _height = 0.0f;

        [SerializeField] protected bool _toggled = false;
        [SerializeField] protected bool _isFrontFace = false;
        [SerializeField] protected bool _ready = false;

        protected RectTransform _rectTransform;
        protected Image _image;
        protected Button _button;
        protected TextMeshProUGUI _text;
        protected GameObject _gameObject;

        [SerializeField] protected Sprite _faceSprite = null;
        [SerializeField] protected Sprite _backSprite = null;

        public ResearchCardAnimation CardAnimation { get { return this._cardAnimation; } }

        public float Width { get { return this._width; } }
        public float Height { get { return this._height; } }

        public bool Toggled { get { return this._toggled; } }
        public bool IsFrontFace { get { return this._isFrontFace; } }
        public bool Ready { get { return this._ready; } set { this._ready = value; if(value) this._research.CardsReady++; } }

        public UnitClassType ClassType { get { return this._classType; } }
        public UnitType UnitType { get { return this._unitType; } }
        public UnitUpgradeType UpgradeType { get { return this._upgradeType; } }

        public RectTransform rectTransform { get { return this._rectTransform; } }
        public Image image { get { return this._image; } }

        #endregion

        #region UNITY
        public void OnPointerUp(PointerEventData eventData) {
            if(this._cardAnimation.State == CardState.FINISHED)
                this.Clicked();
        }
        #endregion

        #region CLASS

        public virtual void Init(Research parent, Sprite FaceSprite, Sprite backSprite, Vector3 pos, UnitClassType classType = UnitClassType.NONE, UnitType unitType = UnitType.NONE, int keyID = -1) {

            this._rectTransform = this.transform as RectTransform;
            this._gameObject = this.gameObject as GameObject;
            this._image = this.transform.GetComponent<Image>() as Image;
            this._button = this.transform.GetComponent<Button>() as Button;
            this._cardAnimation = this.transform.GetComponent<ResearchCardAnimation>() as ResearchCardAnimation;

            this._width = this._rectTransform.sizeDelta.x;
            this._height = this._rectTransform.sizeDelta.y;

            GameObject temp = this.transform.Find("Text").gameObject;
            this._text = temp.GetComponent<TextMeshProUGUI>() as TextMeshProUGUI;
            this._text.text = string.Empty;

            this._research = parent;

            this._faceSprite = FaceSprite;
            this._backSprite = backSprite;

            this._classType = classType;
            this._unitType = unitType;
            this._keyID = keyID;

            this._rectTransform.anchoredPosition = pos;

            this._isFrontFace = false;
            if(this._image != null)
                this._image.sprite = this._backSprite;

            this._cardAnimation.Init(this);
        }

        public virtual void Clicked() {
            this._research.Controller.playerSound.PlayResearchCardTurn();
            StartCoroutine(this._cardAnimation.RotateAndFade());
        }

        public virtual void Finished() {
            this._research.SelectedCard(this._keyID, this._classType, this._unitType, this._upgradeType);
        }

        public virtual void DisplayCard() {
            this._toggled = true;

            this._gameObject.SetActive(true);
        }

        public virtual void HideCard() {
            this.ResetCard();

            this._gameObject.SetActive(false);
        }

        public virtual void SetPosition(Vector3 pos) {
            //this._researchAnimation.PlaySpawnAnimation();

            if(this._rectTransform.anchoredPosition == ((Vector2)pos))
                return;
            else
                this._rectTransform.anchoredPosition = pos;

        }

        public virtual void ChangeFace() {
            if(this._isFrontFace) {

                this._isFrontFace = false;
                this._image.sprite = this._backSprite;

            } else {

                this._isFrontFace = true;
                this._image.sprite = this._faceSprite;

            }
        }

        public virtual void ResetCard() {
            this._toggled = false;
            this._ready = false;

            if(this._isFrontFace)
                this.ChangeFace();

            this._text.gameObject.SetActive(false);
            this._image.color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            this._rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        #endregion
    }

}
