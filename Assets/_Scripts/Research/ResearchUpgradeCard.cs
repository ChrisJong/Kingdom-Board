namespace KingdomBoard.Research {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;
    using Scriptable;

    public sealed class ResearchUpgradeCard : ResearchCard {

        #region VARIABLE

        private UpgradeScriptable _data = null;
        private ResearchUpgradeData _upgradeData = null;

        #endregion

        #region CLASS

        public void Init(Research parent, UpgradeScriptable data, ResearchUpgradeData upgradeData, Vector3 pos, int keyID = -1) {
            this.Init(parent, data.researchCardFaceSprite, data.researchCardBackSprite, pos, data.classType, UnitType.NONE, keyID);

            /*if(this._image != null)
                this._image.sprite = this._faceSprite;*/
            
            this._upgradeType = data.upgradeType;
            this._text.text = "+" + data.value;
            this._data = data;
            this._upgradeData = upgradeData;
        }

        public void UpdateCard() {
            this._text.text = "+" + this._upgradeData.CurrentValue + "\r\n" +
                              "Times Upgraded: " + this._upgradeData.ResearchCount.ToString();
        }

        public void ActivateText(bool enabled) {
            this._text.gameObject.SetActive(enabled);
        }

        public override void Clicked() {

            this.ActivateText(false);

            this._cardAnimation.PlayFadeAnimation();
            //UnityEditor.EditorApplication.isPaused = true;
            this.Finished();
        }
        #endregion
    }
}