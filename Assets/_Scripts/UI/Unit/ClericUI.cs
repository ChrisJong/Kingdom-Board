namespace UI {

    using UnityEngine;
    using UnityEngine.UI;

    using Constants;
    using Enum;
    using Unit;

    public class ClericUI : UnitUI {
        #region VARIABLE
        public Button _btnHeal;

        private bool _healing = false;
        #endregion

        #region UNITY
        protected override void Awake() {
            base.Awake();

            if(this._btnHeal == null)
                this._btnHeal = this.FindButton(this._tSelected, UIValues.Unit.HEALBUTTON);
        }

        protected override void OnEnable() {
            base.OnEnable();

            this._btnHeal.onClick.AddListener(this.InitiateHeal);
        }

        protected override void OnDisable() {
            base.OnDisable();

            this._btnHeal.onClick.RemoveListener(this.InitiateHeal);
        }
        #endregion

        #region CLASS
        public void FinishHeal() {
            Debug.Log("Finish Healing");
            var unitScript = (Cleric)this.unit;

            this.controller.selectionState = SelectionState.FREE;

            unitScript.FinishHealing();
            unitScript.radiusDrawer.TurnOff();

            this.ResetUI();
        }

        protected override void ResetUI() {
            base.ResetUI();

            this._btnHeal.gameObject.SetActive(true);
        }

        protected override void Cancel() {
            base.Cancel();

            if(this._healing) {
                this.ResetUI();
                this._healing = false;
                this.controller.selectionState = SelectionState.FREE;
                this.unit.radiusDrawer.TurnOff();
            }
        }

        private void InitiateHeal() {
            Cleric unitScript = this.unit as Cleric;

            if(!unitScript.canHeal) {
                Debug.Log(unit.name + "Can Not Heal Anymore.");
                return;
            }

            Debug.Log("BEGIN HEALING");
            this._healing = true;
            this.controller.playerSelection.lockSelection = true;
            this.controller.selectionState = SelectionState.SELECT_ALLYTARGET;

            this.unit.radiusDrawer.TurnOn();
            this.unit.unitState = UnitState.HEAL_STANDBY;
            this.unit.radiusDrawer.DrawSpecialRadius(unitScript.healingRadius);

            this._btnCancel.gameObject.SetActive(true);
            this._btnAttack.gameObject.SetActive(false);
            this._btnHeal.gameObject.SetActive(false);
            this._btnMove.gameObject.SetActive(false);
            this._btnEnd.gameObject.SetActive(false);
        }
        #endregion
    }
}