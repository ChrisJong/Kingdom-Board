namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Constants;
    using Enum;

    public class PlayerUI : ScreenSpaceUI {

        #region VARIABLE
        public Text textInfo;
        public Text textTimer;
        public Text debugInfo;
        public Button btnEnd;

        protected Transform _tPersistance;
        protected GameObject _goPersistance;

        [SerializeField] protected List<Transform> _childrenList = new List<Transform>();

        private GameObject _bannerGroup = null;
        private PlayerBanner _banner = null;

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
            this.textTimer = this._tPersistance.Find("Timer_TEXT").GetComponent<Text>();

            this._bannerGroup = this._goUI.transform.Find("Banner").gameObject;
            this._banner = this._bannerGroup.GetComponent<PlayerBanner>() as PlayerBanner;
            this._banner.Init(this);

            foreach(Transform temp in this._goUI.GetComponentInChildren<Transform>()) {
                if(temp.GetHashCode() == this._bannerGroup.transform.GetHashCode())
                    continue;
                this._childrenList.Add(temp);
            }
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
            //this._goUI.SetActive(true);

            foreach(Transform temp in this._childrenList) {
                if(temp.gameObject.activeSelf)
                    continue;

                temp.gameObject.SetActive(true);
            }

            this.UpdateInfo();
        }

        public void UpdareTimer(float time) {
            this.textTimer.text = Mathf.Round(time).ToString();
        }

        public override void Hide() {
            //this._goUI.SetActive(false);

            this._bannerGroup.gameObject.SetActive(false);

            foreach(Transform temp in this._childrenList) {
                if(!temp.gameObject.activeSelf)
                    continue;

                temp.gameObject.SetActive(false);
            }
        }

        protected override void ResetUI() {
            throw new System.NotImplementedException();
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

        public void ShowBanner() {
            if(!this._bannerGroup.activeSelf)
                this._bannerGroup.SetActive(true);

            this._banner.SwapBanner(this.controller.isAttacking);

            this._banner.StartBannerAnimation();
        }

        public void FinishedBannerAnim() {
            this.controller.StartTurn();
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