namespace UI {

    using System;

    using UnityEngine;
    using UnityEngine.UI;

    using Constants;
    using Enum;
    using Unit;

    public abstract class UnitUI : ScreenSpaceUI {

        #region VARIABLE
        public UnitBase unit;

        public Text _textInfo;

        public Button _btnEnd;
        public Button _btnAttack;
        public Button _btnMove;
        public SpriteRenderer _spCircle;

        private bool _showAttack = false;
        private bool _showMove = false;
        #endregion
        
        #region UNITY
        protected override void Awake() {
            if(this.unit == null) {
                this.unit = this.transform.GetComponent<UnitBase>();
                this.controller = this.unit.controller;
            }

            this.FindUI(this.transform, UIValues.Unit.UNITUI);
            base.Awake();

            if(this._spCircle == null)
                this._spCircle = this.transform.Find(UIValues.Unit.SELECTIONCIRCLE).GetComponent<SpriteRenderer>();
            this._spCircle.gameObject.SetActive(false);

            if(this._btnEnd == null)
                this._btnEnd = this.FindButton(this._tSelected, UIValues.Unit.ENDBUTTON);


            if(this._btnAttack == null)
                this._btnAttack = this.FindButton(this._tSelected, UIValues.Unit.ATTACKBUTTON);


            if(this._btnMove == null)
                this._btnMove = this.FindButton(this._tSelected, UIValues.Unit.MOVEBUTTON);


            if(this._textInfo == null)
                this._textInfo = this._tHover.Find(UIValues.Unit.INFOTEXT).GetComponent<Text>();
        }

        protected virtual void OnEnable() {
            if(this.controller == null)
                this.controller = this.unit.controller;

            this._btnEnd.onClick.AddListener(this.Back);
            this._btnAttack.onClick.AddListener(this.InitiateAttack);
            this._btnMove.onClick.AddListener(this.InitiateMove);
        }

        protected virtual void OnDisable() {
            this._btnEnd.onClick.RemoveListener(this.Back);
            this._btnAttack.onClick.RemoveListener(this.InitiateAttack);
            this._btnMove.onClick.RemoveListener(this.InitiateMove);
        }

        protected override void OnMouseEnter() {
            base.OnMouseEnter();

            this._spCircle.gameObject.SetActive(true);
        }

        protected override void OnMouseExit() {
            base.OnMouseExit();

            this._spCircle.gameObject.SetActive(false);
        }
        #endregion

        #region CLASS
        public void Init() {
            if(this.controller != null)
                this._spCircle.color = this.controller.color;

            this.UpdateInfo();
        }

        public override void UpdateUI() {
            base.UpdateUI();
        }
        public override void Display() {
            base.Display();

            if(this._goSelected.activeSelf)
                return;

            this._goSelected.SetActive(true);
        }

        public override void Hide() {
            base.Hide();

            if(!this._goSelected.activeSelf)
                return;

            this.ResetUI();
            this._goSelected.SetActive(false);

            if(this._goHover.activeSelf)
                this._goHover.SetActive(false);
        }

        protected override void ResetUI() {
            this._showAttack = false;
            this._showMove = false;
        }

        private void UpdateInfo() {
            string text;
            string state;

            if(this._showAttack)
                state = "ATTACKING - SELECT A UNIT TO ATTACK";
            else if(this._showMove)
                state = "MOVE - SELECT A POSITION TO MOVE TO";
            else if(this.unit.hasFinished)
                state = "FINISHED";
            else
                state = "IDLE";

            text = "Health: " + this.unit.currentHealth.ToString() + "\r\n" +
                   "Attack: " + this.unit.GetDamage().ToString() + "\r\n" +
                   "Resistance: " + this.unit.resistance.ToString() + " - " + this.unit.resistancePercentage.ToString() + "%" + "\r\n" +
                   "Weakness: " + this.unit.weakness.ToString() + " - " + this.unit.weaknessPercentage.ToString() + "%" + "\r\n" +
                   "Current State: " + state;

            this._textInfo.text = text;
        }

        private void InitiateAttack() {
            Debug.Log("BEGIN ATTACK");
            this._showAttack = true;
        }

        private void InitiateMove() {
            Debug.Log("BEGIN MOVE");
            this._showMove = true;
        }

        private void Back() {
        }
        #endregion
    }
}