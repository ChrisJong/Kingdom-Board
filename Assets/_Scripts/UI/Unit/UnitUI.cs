namespace UI {

    using System;

    using UnityEngine;
    using UnityEngine.UI;

    using Unit;

    public class UnitUI : ScreenSpaceUI {

        #region VARIABLE
        public UnitBase unit;

        public Button btnEnd;
        public Button btnAttack;
        public Button btnMove;

        #endregion
        
        #region UNITY
        private void Awake() {
            if(this.unit == null)
                this.unit = this.transform.parent.gameObject.GetComponent<UnitBase>();

            this.controller = this.unit.controller;
        }
        #endregion

        #region CLASS
        public override void Display() {
            throw new NotImplementedException();
        }

        public override void Hide() {
            throw new NotImplementedException();
        }

        protected override void Reset() {
            throw new NotImplementedException();
        }
        #endregion
    }
}