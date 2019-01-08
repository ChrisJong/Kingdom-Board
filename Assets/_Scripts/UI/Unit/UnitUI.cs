namespace UI {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Constants;
    using Enum;
    using Unit;

    public abstract class UnitUI : ScreenSpace {

        #region VARIABLE
        public UnitBase unit;

        public Text _textInfo;

        public Button _btnEnd;
        public Button _btnCancel;
        public Button _btnAttack;
        public Button _btnMove;
        public Button _btnFinishMove;

        [SerializeField]
        protected List<Material> _unitMaterials;

        protected bool _attacking = false;
        protected bool _moving = false;
        #endregion
        
        #region UNITY
        #endregion

        #region CLASS
        public override void Init() {
            this.UpdateInfo();
        }

        public override void UpdateUI() {
            this.UpdateInfo();
        }

        public override void DisplayUI() {
        }

        public override void HideUI() {
        }

        public override void ResetUI() {
        }

        protected virtual void ActivateOutline(Color color, float width = 0.03f) {
            foreach(Material mat in this._unitMaterials) {
                mat.SetFloat("_Outline", width);
                mat.SetColor("_OutlineColor", color);
            }
        }

        protected virtual void DeactivateOutline() {
            foreach(Material mat in this._unitMaterials) {
                mat.SetFloat("_Outline", 0.0f);
            }
        }

        protected void InitiateAttack() {
            if(!this.unit.canAttack) {
                Debug.Log("Unit - " + unit.name + " Can NOT Attack Anymore");
                return;
            }

            //Debug.Log("BEGIN ATTACK");
            this._attacking = true;
            this.controller.playerSelection.lockSelection = true;
            this.controller.selectionState = SelectionState.SELECT_ENEMYTARGET;

            this.unit.radiusDrawer.TurnOn();
            this.unit.unitState = UnitState.ATTACK_STANDBY;
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
            this.controller.playerSelection.lockSelection = true;
            this.controller.selectionState = SelectionState.SELECT_POINT;

            this.unit.unitState = UnitState.MOVING_STANDBY;
            this.unit.lastPosition = this.unit.position;

            this._btnCancel.gameObject.SetActive(true);
            this._btnFinishMove.gameObject.SetActive(true);
            this._btnAttack.gameObject.SetActive(false);
            this._btnMove.gameObject.SetActive(false);
            this._btnEnd.gameObject.SetActive(false);
        }

        public virtual void FinishMove() {
            Debug.Log("Finish Movement");

            this.controller.selectionState = SelectionState.FREE;

            this.unit.FinishMove();
            this.unit.radiusDrawer.TurnOff();

            this.ResetUI();
        }

        public virtual void FinishAttack() {
            Debug.Log("Finish Attack");
            this.controller.selectionState = SelectionState.FREE;
            this.unit.radiusDrawer.TurnOff();
            this.ResetUI();
        }

        protected virtual void Cancel() {
            if(this._btnCancel == null)
                throw new ArgumentNullException("Missing Cancel Button");

            if(this._attacking) {
                this.ResetUI();
                this._attacking = false;
                this.controller.selectionState = SelectionState.FREE;
                this.unit.radiusDrawer.TurnOff();
                this.unit.unitState = UnitState.IDLE;
            } else if(this._moving) {
                this.ResetUI();
                this._moving = false;
                this.controller.selectionState = SelectionState.FREE;
                this.unit.radiusDrawer.TurnOff();
                this.unit.unitState = UnitState.IDLE;
                this.unit.CancelMove();
            }
        }

        protected void End() {
            this.unit.Finished();
            this.ResetUI();
            this.UpdateInfo();
            this.HideUI();
        }

        private void UpdateInfo() {
            string text;
            string state;

            if(this._attacking)
                state = "ATTACKING - SELECT A UNIT TO ATTACK";
            else if(this._moving)
                state = "MOVE - SELECT A POSITION TO MOVE TO";
            else if(this.unit.unitState == UnitState.FINISH)
                state = "FINISHED";
            else
                state = "IDLE";

            text = "Health: " + this.unit.currentHealth + " / " + this.unit.maxHealth + "\r\n" +
                   "Attack: " + this.unit.GetDamage() + "\r\n" +
                   "Resistance: " + this.unit.resistanceType.ToString() + " - " + this.unit.resistancePercentage.ToString() + "%" + "\r\n" +
                   "Weakness: " + this.unit.weaknessType.ToString() + " - " + this.unit.weaknessPercentage.ToString() + "%" + "\r\n" +
                   "Current State: " + state;

            this._textInfo.text = text;
        }
        #endregion
    }
}