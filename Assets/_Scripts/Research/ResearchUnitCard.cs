namespace Research {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.EventSystems;

    using Enum;
    using Scriptable;
    using Unit;

    public sealed class ResearchUnitCard : ResearchCard {

        #region VARIABLE

        [SerializeField] private UnitScriptable _data;

        [SerializeField] private int _classTier = 0;

        [SerializeField] private bool _unlocked = false;

        public int ClassTier {
            get { return this._classTier; }
        }

        public bool Unlocked {
            get { return this._unlocked; }
        }

        #endregion

        #region CLASS
        public void Init(Research parent, UnitScriptable data, Vector3 pos, int keyID = -1) {
            this.Init(parent, data.cardFaceSprite, data.cardBackSprite, pos, data.classType, data.unitType, keyID);

            this._classTier = data.classTier;

            if(this._image != null)
                this._image.sprite = this._faceSprite;
        }

        public override void Clicked() {

            if(this._unitType != UnitType.NONE)
                this._unlocked = true;

            base.Clicked();
        }
        #endregion
    }
}