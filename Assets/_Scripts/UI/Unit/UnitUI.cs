﻿namespace UI {

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
        public Button _btnCancel;
        public Button _btnAttack;
        public Button _btnMove;
        public Button _btnFinishMove;
        public SpriteRenderer _spCircle;

        private bool _attacking = false;
        private bool _moving = false;
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

            if(this._btnCancel == null)
                this._btnCancel = this.FindButton(this._tSelected, UIValues.Unit.CANCELBUTTON);
            this._btnCancel.gameObject.SetActive(false);

            if(this._btnAttack == null)
                this._btnAttack = this.FindButton(this._tSelected, UIValues.Unit.ATTACKBUTTON);

            if(this._btnMove == null)
                this._btnMove = this.FindButton(this._tSelected, UIValues.Unit.MOVEBUTTON);

            if(this._btnFinishMove == null)
                this._btnFinishMove = this.FindButton(this._tSelected, "Finish_BTN");

            if(this._textInfo == null)
                this._textInfo = this._tHover.Find(UIValues.Unit.INFOTEXT).GetComponent<Text>();
        }

        protected virtual void OnEnable() {
            if(this.controller == null)
                this.controller = this.unit.controller;

            this._btnEnd.onClick.AddListener(this.End);
            this._btnCancel.onClick.AddListener(this.Cancel);
            this._btnAttack.onClick.AddListener(this.InitiateAttack);
            this._btnMove.onClick.AddListener(this.InitiateMove);
            this._btnFinishMove.onClick.AddListener(this.FinishMove);
        }

        protected virtual void OnDisable() {
            this._btnEnd.onClick.RemoveListener(this.End);
            this._btnCancel.onClick.RemoveListener(this.Cancel);
            this._btnAttack.onClick.RemoveListener(this.InitiateAttack);
            this._btnMove.onClick.RemoveListener(this.InitiateMove);
            this._btnFinishMove.onClick.RemoveListener(this.FinishMove);
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

            this.UpdateInfo();
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

        public virtual void FinishAttack() {
            Debug.Log("Finish Attack");
            this.controller.selectionState = SelectionState.FREE;
            this.unit.radiusDrawer.TurnOff();
            this.ResetUI();
        }

        protected override void ResetUI() {
            this._btnCancel.gameObject.SetActive(false);
            this._btnFinishMove.gameObject.SetActive(false);
            this._btnAttack.gameObject.SetActive(true);
            this._btnMove.gameObject.SetActive(true);
            this._btnEnd.gameObject.SetActive(true);
            this._attacking = false;
            this._moving = false;
        }

        protected void InitiateAttack() {
            if(!this.unit.canAttack) {
                Debug.Log("Unit - " + unit.name + " Can NOT Attack Anymore");
                return;
            }

            Debug.Log("BEGIN ATTACK");
            this._attacking = true;
            this.controller.selectionState = SelectionState.UNIT_ATTACK;

            this.unit.radiusDrawer.TurnOn();
            this.unit.radiusDrawer.DrawAttackRadius(this.unit.attackRadius);

            this._btnCancel.gameObject.SetActive(true);
            this._btnAttack.gameObject.SetActive(false);
            this._btnMove.gameObject.SetActive(false);
            this._btnEnd.gameObject.SetActive(false);
        }

        protected void InitiateMove() {
            if(!this.unit.canMove) {
                Debug.Log("Unit - " + unit.name + " Can NOT Move Anymore");
                return;
            }

            Debug.Log("BEGIN MOVE");
            this._moving = true;
            this.controller.selectionState = SelectionState.UNIT_MOVE;

            this.unit.radiusDrawer.TurnOn();
            this.unit.radiusDrawer.DrawMoveRadius(this.unit.moveRadius);
            this.unit.lastPosition = this.unit.position;

            this._btnCancel.gameObject.SetActive(true);
            this._btnFinishMove.gameObject.SetActive(true);
            this._btnAttack.gameObject.SetActive(false);
            this._btnMove.gameObject.SetActive(false);
            this._btnEnd.gameObject.SetActive(false);
        }

        protected void FinishMove() {
            Debug.Log("Finish Movement");

            this.controller.selectionState = SelectionState.FREE;

            this.unit.FinishMove();
            this.unit.radiusDrawer.TurnOff();

            ResetUI();
        }

        protected void Cancel() {
            if(this._btnCancel == null)
                throw new ArgumentNullException("Missing Cancel Button");

            if(this._attacking) {
                this.ResetUI();
                this._attacking = false;
                this.controller.selectionState = SelectionState.FREE;
                this.unit.radiusDrawer.TurnOff();
            } else if(this._moving) {
                this.ResetUI();
                this._moving = false;
                this.controller.selectionState = SelectionState.FREE;
                this.unit.radiusDrawer.TurnOff();
                this.unit.CancelMove();
            } else {
                Debug.LogError("Cancel Button Shouldn't be shown, something went wrong");
            }
        }

        protected void End() {
            this.unit.Finished();
            this.ResetUI();
            this.UpdateInfo();
            this.Hide();
        }

        private void UpdateInfo() {
            string text;
            string state;

            if(this._attacking)
                state = "ATTACKING - SELECT A UNIT TO ATTACK";
            else if(this._moving)
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
        #endregion
    }
}