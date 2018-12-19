namespace Research {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;
    using Scriptable;

    public sealed class ResearchUpgradeCard : ResearchCard {

        #region VARIABLE

        [SerializeField] UpgradeScriptable _data;

        #endregion

        #region CLASS

        public void Init(Research parent, UpgradeScriptable data, Vector3 pos, int keyID = -1) {
            this.Init(parent, data.cardFaceSprite, data.cardBackSprite, pos, data.classType, UnitType.NONE, keyID);

            if(this._image != null)
                this._image.sprite = this._faceSprite;
        }

        #endregion
    }
}