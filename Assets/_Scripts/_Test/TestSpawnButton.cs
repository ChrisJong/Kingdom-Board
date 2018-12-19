namespace Test {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    using Enum;
    using System;

    public class TestSpawnButton : MonoBehaviour, IPointerUpHandler {

        [SerializeField] private bool isLocked = false;

        [SerializeField] private Button _button;
        [SerializeField] private Image _image;

        [SerializeField] private UnitType _unitType;
        [SerializeField] private ClassType _classType;

        [SerializeField] private Sprite _lockedSprite;
        [SerializeField] private Sprite _unlockedSprite;

        public ClassType ClassType {
            get { return this._classType; }
        }

        public UnitType UnitType {
            get { return this._unitType; }
        }

        #region UNITY
        public void OnPointerUp(PointerEventData eventData) {
            if(this.isLocked)
                Debug.Log(this._unitType.ToString() + "_BTN Is Locked");
            else {
                Debug.Log(this._unitType.ToString() + "_BTN Is UnLocked");
            }
        }

        #endregion

        private void Awake() {
            if(this._button == null)
                this._button = this.GetComponent<Button>() as Button;

            if(this._image == null)
                this._image = this.GetComponent<Image>() as Image;
        }

        public void Init() {

        }

        public void Init(UnitType unitType, ClassType classType, Sprite unLockedSprite, bool locked = false) {
            this._unitType = unitType;
            this._classType = classType;

            this._unlockedSprite = unLockedSprite;

            if(locked) {
                if(this._lockedSprite != null)
                    this._image.sprite = this._lockedSprite;
            } else {
                this._image.sprite = this._unlockedSprite;
            }
        }

        public void Init(UnitType unitType, ClassType classType, Sprite lockedSprite, Sprite unLockedSprite, bool locked = false) {
            this._unitType = unitType;
            this._classType = classType;

            this._lockedSprite = lockedSprite;
            this._unlockedSprite = unLockedSprite;

            if(locked) {
                this._image.sprite = this._lockedSprite;
            } else {
                this._image.sprite = this._unlockedSprite;
            }
        }

        public void Lock() {
            this.isLocked = true;

            this._image.sprite = this._lockedSprite;
        }

        public void UnLock() {
            this.isLocked = false;

            this._image.sprite = this._unlockedSprite;
        }
    }
}