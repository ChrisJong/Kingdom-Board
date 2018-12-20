namespace Research {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    using Enum;
    using System;

    [RequireComponent(typeof(ResearchCardAnimation))]
    public abstract class ResearchCard : MonoBehaviour, IPointerUpHandler {

        #region VARIABLE

        [SerializeField] protected Research _research;
        [SerializeField] protected ResearchCardAnimation _researchAnimation;

        [SerializeField] protected ClassType _classType = ClassType.NONE;
        [SerializeField] protected UnitType _unitType = UnitType.NONE;
        [SerializeField] protected UnitUpgradeType _upgradeType = UnitUpgradeType.NONE;

        [SerializeField] protected int _keyID = -1; // -1 Doesn't belong in anything.

        [SerializeField] protected bool _toggled = false;

        protected RectTransform _rectTransform;
        protected Image _image;
        protected Button _button;
        [SerializeField] protected Text _text;
        protected GameObject _gameObject;

        [SerializeField] protected Sprite _faceSprite = null;
        [SerializeField] protected Sprite _backSprite = null;

        public bool Toggled {
            get { return this._toggled; }
        }

        public ClassType ClassType {
            get { return this._classType; }
        }

        public UnitType UnitType {
            get { return this._unitType; }
        }

        #endregion

        #region UNITY
        public void OnPointerUp(PointerEventData eventData) {
            this.Clicked();
        }
        #endregion

        #region CLASS

        public virtual void Init(Research parent, Sprite FaceSprite, Sprite backSprite, Vector3 pos, ClassType classType = ClassType.NONE, UnitType unitType = UnitType.NONE, int keyID = -1) {

            this._rectTransform = this.transform as RectTransform;
            this._gameObject = this.gameObject as GameObject;
            this._image = this.transform.GetComponent<Image>() as Image;
            this._button = this.transform.GetComponent<Button>() as Button;
            this._researchAnimation = this.transform.GetComponent<ResearchCardAnimation>() as ResearchCardAnimation;

            GameObject temp = this.transform.Find("Text").gameObject;
            this._text = temp.GetComponent<Text>() as Text;

            this._research = parent;

            this._faceSprite = FaceSprite;
            this._backSprite = backSprite;

            this._classType = classType;
            this._unitType = unitType;
            this._keyID = keyID;

            this._rectTransform.anchoredPosition = pos;

            this._researchAnimation.Init(this);
        }

        public virtual void Clicked() {
            this._research.SelectedCard(this._keyID, this._classType, this._unitType, this._upgradeType);
        }

        public virtual void DisplayCard() {
            this._toggled = true;

            this._gameObject.SetActive(true);
        }

        public virtual void HideCard() {
            this._toggled = false;

            this._gameObject.SetActive(false);
        }

        public virtual void SetPosition(Vector3 pos) {
            //this._researchAnimation.PlaySpawnAnimation();

            if(this._rectTransform.anchoredPosition == ((Vector2)pos))
                return;
            else
                this._rectTransform.anchoredPosition = pos;

        }
        #endregion
    }

}
