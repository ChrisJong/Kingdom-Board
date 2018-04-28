﻿namespace UI {

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
            this.FindUI(this.transform, UIValues.Player.PLAYERUI);

            if(this.tUI.Find(UIValues.PERSISTANCEGROUP) != null) {
                this._tPersistance = this.tUI.Find(UIValues.PERSISTANCEGROUP);
                this._goPersistance = this._tPersistance.gameObject;

                this.debugInfo = this.tUI.Find("Debug_TEXT").GetComponent<Text>();
                this.debugInfo.text = "";
            }

            if(this._tPersistance.Find(UIValues.Player.ENDBUTTON) != null)
                this.btnEnd = this._tPersistance.Find(UIValues.Player.ENDBUTTON).GetComponent<Button>() as Button;
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
        }
        #endregion

        #region CLASS
        public override void Display() {
            this._goUI.SetActive(true);
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
                
            }
        }

        public void ChangeDebugText(string text) {
            string temp = "Debug: " + text;

            this.debugInfo.text = temp;
        }

        private void EndTurn() {
            this.controller.EndTurn();
            this.Hide();
        }
        #endregion
    }
}