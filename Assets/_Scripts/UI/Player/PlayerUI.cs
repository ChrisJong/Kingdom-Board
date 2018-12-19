namespace UI {

    using System;

    using UnityEngine;
    using UnityEngine.UI;

    using Constants;
    using Enum;

    public class PlayerUI : ScreenSpaceUI {

        #region VARIABLE
        public Text textInfo;
        public Text debugInfo;
        public Button btnEnd;

        protected Transform _tPersistance;
        protected GameObject _goPersistance;
        #endregion

        #region UNITY
        protected override void Awake() {
            this.FindUI(this.transform, UIValues.UISUFFIX);

            if(this.tUI.Find(UIValues.PERSISTANCEGROUP) != null) {
                this._tPersistance = this.tUI.Find(UIValues.PERSISTANCEGROUP);
                this._goPersistance = this._tPersistance.gameObject;

                this.debugInfo = this.tUI.Find("Debug_TEXT").GetComponent<Text>();
                this.debugInfo.text = "";
            }

            if(this._tPersistance.Find(UIValues.Player.ENDBUTTON) != null)
                this.btnEnd = this._tPersistance.Find(UIValues.Player.ENDBUTTON).GetComponent<Button>();

            this.textInfo = this._tPersistance.Find("Info_TEXT").GetComponent<Text>();
        }

        private void OnEnable() {
            this.btnEnd.onClick.AddListener(this.EndTurn);
        }

        private void OnDisable() {
            this.btnEnd.onClick.RemoveListener(this.EndTurn);
        }

        public void Update() {
            if(controller.selectionState != SelectionState.FREE)
                this.btnEnd.gameObject.SetActive(false);
            else
                this.btnEnd.gameObject.SetActive(true);

            this.UpdateInfo();
        }
        #endregion

        #region CLASS
        public override void Display() {
            this._goUI.SetActive(true);

            this.UpdateInfo();
        }

        public override void Hide() {
            this._goUI.SetActive(false);
        }

        protected override void ResetUI() {
            throw new NotImplementedException();
        }

        public override void UpdateUI() {
            if(this.controller.turnEnded) {
                this.Hide();
            } else {
                this.UpdateInfo();
            }
        }

        public void ChangeDebugText(string text) {
            string temp = "Debug: " + text;

            this.debugInfo.text = temp;
        }

        private void EndTurn() {
            this.Hide();
            this.controller.EndTurn();
        }

        private void UpdateInfo() {
            string text = string.Empty;

            text = "GOLD: " + this.controller.CurrentGold.ToString() + "\r\n" +
                   "UNIT CAP: " + this.controller.CurrentUnitCap.ToString() + " / " + this.controller.MaxUnitCap + "\r\n" + 
                   "PHASE: " + this.controller.state.ToString();

            this.textInfo.text = text;
        }
        #endregion
    }
}