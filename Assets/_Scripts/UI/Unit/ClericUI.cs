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

        #endregion

        #region CLASS
        public void FinishHeal() {
            Debug.Log("Finish Healing");
            var unitScript = this._unitBase as Cleric;

            this.Controller.playerSelect.CurrentState = SelectionState.FREE;

            this.ResetUI();
        }

        public override void ResetUI() {
            base.ResetUI();

            this._btnHeal.gameObject.SetActive(true);
        }

        protected override void Cancel() {
            base.Cancel();

            if(this._healing) {
                this.ResetUI();
                this._healing = false;
                this.Controller.playerSelect.CurrentState = SelectionState.FREE;
            }
        }

        private void InitiateHeal() {
        }
        #endregion
    }
}