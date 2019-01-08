namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using TMPro;

    using Enum;
    using Player;

    public class PlayerUI : ScreenSpace {

        #region VARIABLE

        [SerializeField] private PlayerEndButton _endTurnButton = null;
        [SerializeField] private PlayerBanner _playerBanner = null;

        [SerializeField] private GameObject _displayGroup = null;
        private RectTransform _displayGroupTransform = null;

        [SerializeField] private TextMeshProUGUI _infoText;

        public GameObject bannerGroup = null;
        public GameObject spawnGroup = null;
        public GameObject researchGroup = null;

        [SerializeField] protected List<Transform> _uiChildrenList = new List<Transform>();

        #endregion

        #region UNITY

        public void Update() {

            if(this.controller.state == PlayerState.ATTACKING || this.controller.state == PlayerState.DEFENDING) {
                this._endTurnButton.UpdateButton();
                this.UpdateInfo();
            }
        }

        #endregion

        #region CLASS
        public override void Init(Player controller) {
            base.Init(controller);

            if(this._displayGroup == null)
                this._displayGroup = this._uiGroupRectTransform.Find("_Display").gameObject;
            else
                this._displayGroupTransform = this._displayGroup.transform as RectTransform;

            if(this._infoText == null)
                this._infoText = this._displayGroupTransform.Find("info_TEXT").GetComponent<TextMeshProUGUI>() as TextMeshProUGUI;

            if(this._endTurnButton == null) {
                this._endTurnButton = this._displayGroupTransform.Find("End_BTN").GetComponent<PlayerEndButton>() as PlayerEndButton;
                this._endTurnButton.Init(this);
            } else
                this._endTurnButton.Init(this);

            if(this.bannerGroup == null) {
                this.bannerGroup = this._uiGroup.transform.Find("Banner").gameObject;
                this._playerBanner = this.bannerGroup.GetComponent<PlayerBanner>() as PlayerBanner;
                this._playerBanner.Init(this);
            } else if(this._playerBanner == null) {
                this._playerBanner = this.bannerGroup.GetComponent<PlayerBanner>() as PlayerBanner;
                this._playerBanner.Init(this);
            } else 
                this._playerBanner.Init(this);

            foreach(Transform temp in this._uiGroup.GetComponentInChildren<Transform>()) {
                if(temp.GetHashCode() == this.bannerGroup.transform.GetHashCode())
                    continue;

                if(temp.name == "Spawn") {
                    this.spawnGroup = temp.gameObject;
                }

                if(temp.name == "Research")
                    this.researchGroup = temp.gameObject;

                this._uiChildrenList.Add(temp);
            }

        }

        public override void DisplayUI() {
            //this._goUI.SetActive(true);

            foreach(Transform temp in this._uiChildrenList) {
                if(temp.gameObject.activeSelf)
                    continue;

                temp.gameObject.SetActive(true);
            }

            this.UpdateInfo();
        }

        public override void HideUI() {
            //this._goUI.SetActive(false);

            this.bannerGroup.gameObject.SetActive(false);

            foreach(Transform temp in this._uiChildrenList) {
                if(!temp.gameObject.activeSelf)
                    continue;

                temp.gameObject.SetActive(false);
            }
        }

        public override void ResetUI() {
            throw new System.NotImplementedException();
        }

        public override void UpdateUI() {
            if(this.controller.turnEnded) {
                this.HideUI();
            } else {
                this.UpdateInfo();
            }
        }

        public void ShowBanner() {
            if(!this.bannerGroup.activeSelf)
                this.bannerGroup.SetActive(true);

            this._playerBanner.SwapBanner(this.controller.isAttacking);

            this._playerBanner.StartBannerAnimation();
        }

        public void FinishedBannerAnim() {
            this.controller.StartTurn();
        }

        public void EndTurn() {
            this.HideUI();
            this.controller.EndTurn();
        }

        private void UpdateInfo() {
            string text = string.Empty;

            text = "GOLD: " + this.controller.CurrentGold.ToString() + "\r\n" +
                   "UNIT CAP: " + this.controller.CurrentUnitCap.ToString() + " / " + this.controller.MaxUnitCap + "\r\n" + 
                   "PHASE: " + this.controller.state.ToString();

            this._infoText.text = text;
        }
        #endregion
    }
}