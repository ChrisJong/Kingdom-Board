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

        [SerializeField]
        protected List<Material> _unitMaterials;
        #endregion
        
        #region UNITY
        #endregion

        #region CLASS
        public override void UpdateUI() {
            //this.UpdateInfo();
        }

        public override void DisplayUI() {
        }

        public override void HideUI() {
        }

        public override void OnEnter() {
            //throw new NotImplementedException();
        }

        public override void OnExit() {
            //throw new NotImplementedException();
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
        }

        protected void InitiateMove() {
        }

        public virtual void FinishMove() {
        }

        public virtual void FinishAttack() {
        }

        protected virtual void Cancel() {
        }

        protected void End() {
        }

        private void UpdateInfo() {
        }
        #endregion
    }
}