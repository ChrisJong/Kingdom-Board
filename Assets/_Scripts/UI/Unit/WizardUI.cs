namespace UI {

    using System;

    using UnityEngine;

    using Unit;

    public class WizardUI : UnitUI {
        protected override void Cancel() {
            if(this._btnCancel == null)
                throw new ArgumentNullException("Missing Cancel Button");

            if(this._attacking){
                Wizard temp = this.unit as Wizard;
                temp.splashRadiusDrawer.TurnOff();
            }

            base.Cancel();
        }
    }
}