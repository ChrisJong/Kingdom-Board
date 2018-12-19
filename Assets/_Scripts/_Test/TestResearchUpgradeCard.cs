namespace Test {

    using System;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    using Enum;

    public class TestResearchUpgradeCard : MonoBehaviour, IPointerUpHandler {

        #region VARIABLE
        [SerializeField] TestResearch _parent;
        [SerializeField] TestUpgradeScriptable _upgradeData;

        [SerializeField] private int _keyID = -1;

        [SerializeField] private bool _toggled = false;

        [SerializeField] private Sprite _cardFace;
        [SerializeField] private Sprite _cardBack;

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private Image _image;
        [SerializeField] private Button _button;

        public bool Toggled {
            get { return this._toggled; }
        }

        #endregion

        #region UNITY
        public void OnPointerUp(PointerEventData eventData) {
            this._parent.SelectedCard(ClassType.NONE, UnitType.NONE, this._keyID);
        }
        #endregion

        #region CLASs
        public void Init(TestResearch parent, TestUpgradeScriptable data, Vector3 pos, int keyID) {
            this._rectTransform = this.transform as RectTransform;
            this._gameObject = this.gameObject as GameObject;
            this._image = this._rectTransform.GetComponent<Image>() as Image;
            this._button = this._rectTransform.GetComponent<Button>() as Button;

            this._parent = parent;

            this._upgradeData = data;

            this._keyID = keyID;

            this._cardFace = data.cardFront;
            this._cardBack = data.cardBack;

            this._rectTransform.anchoredPosition = pos;

        }

        public void DisplayCard() {
            this._toggled = true;
            this._gameObject.SetActive(true);
        }

        public void HideCard() {
            this._toggled = false;
            this._gameObject.SetActive(false);
        }

        public void SetPosition(Vector3 pos) {
            if(this._rectTransform.anchoredPosition != ((Vector2)pos))
                this._rectTransform.anchoredPosition = pos;
            else
                return;
        }

        public void DebugCard() {
            Debug.Log("Card Type: " + _upgradeData.upgrade.ToString() + " - Value: " + this._upgradeData.value.ToString());
        }
        #endregion
    }
}
