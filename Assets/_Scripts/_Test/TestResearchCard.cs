namespace Test {
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    using Enum;

    public class TestResearchCard : MonoBehaviour, IPointerUpHandler {

        #region VARIABLE
        [SerializeField] private TestResearch _parent;
        [SerializeField] private TestUnitScriptable _unitData;

        [SerializeField] private bool _toggled = false;
        [SerializeField] private bool _unlocked = false;

        [SerializeField] private int _tierLevel = 0;

        [SerializeField] private ClassType _classType;
        [SerializeField] private UnitType _unitType;

        [SerializeField] private Sprite _cardFace;
        [SerializeField] private Sprite _cardBack;

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;
        [SerializeField] private Button _button;
        [SerializeField] private Text _text;

        public bool Toggled {
            get { return this._toggled; }
        }

        public bool Unlocked {
            get { return this._unlocked; }
        }

        public int TierLevel {
            get { return this._tierLevel; }
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
            //this._toggled = true;

            if(this._unitType != UnitType.NONE) {
                this._unlocked = true;
            }

            this._parent.SelectedCard(this._classType, this._unitType);
        }
        #endregion

        #region CLASS
        public void Init(TestResearch parent, Vector3 pos, ClassType classType, UnitType unitType = UnitType.NONE, int tierLevel = 0) {
            this._rectTransform = this.transform as RectTransform;
            this._image = this.transform.GetComponent<Image>() as Image;
            this._button = this.transform.GetComponent<Button>() as Button;

            this._parent = parent;

            this._classType = classType;
            this._unitType = unitType;

            this._tierLevel = tierLevel;

            this._rectTransform.anchoredPosition = pos;
        }

        public void Init(TestResearch parent, Sprite face, Sprite back, Vector3 pos, ClassType classType, UnitType unitType, int tierLevel = 0) {
            this.Init(parent, pos, classType, unitType, tierLevel);

            this._cardFace = face;
            this._cardBack = back;

            if(this._image != null) {
                this._image.sprite = this._cardFace;
            }
        }

        public void Init(TestResearch parent, TestUnitScriptable unitdata, Vector3 pos, ClassType classType, UnitType unitType = UnitType.NONE, int tierLevel = 0) {
            this.Init(parent, unitdata.cardFace, unitdata.cardBack, pos, classType, unitType, tierLevel);

            this._unitData = unitdata;
        }

        public void DisplayCard() {

            this._toggled = true;

            this.gameObject.SetActive(true);
        }

        public void HideCard() {

            this._toggled = false;

            this.gameObject.SetActive(false);
        }

        public void SetPosition(Vector3 pos) {
            if(this._rectTransform.anchoredPosition == ((Vector2)pos))
                return;
            else
                this._rectTransform.anchoredPosition = pos;
        }

        #endregion
    }
}