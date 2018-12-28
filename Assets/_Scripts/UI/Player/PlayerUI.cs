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

        [SerializeField] float _bannerTimer = 3.5f;
        [SerializeField] float _bannerFadeInTimer = 2.0f;
        [SerializeField] float _bannerFadeOutTimer = 2.0f;
        [SerializeField] float _bannerLimit = 3.5f;
        [SerializeField] float _fadeOutLmit = 2.0f;
        [SerializeField] float _fadeInLImit = 2.0f;

        protected GameObject _banner = null;
        protected Image _bannerImage = null;
        [SerializeField] protected Animation _bannerAnimation = null;

        [SerializeField] private Sprite _attackingSprite;
        [SerializeField] private Sprite _defendingSprite;
        [SerializeField] private Sprite _endSprite;
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

            this._banner = this._goUI.transform.Find("Banner").gameObject;
            this._bannerImage = this._banner.transform.GetComponent<Image>() as Image;
            this._bannerAnimation = this._banner.transform.GetComponent<Animation>() as Animation;

            foreach(Transform temp in this._goUI.GetComponentInChildren<Transform>()) {
                if(temp.GetHashCode() == this._banner.transform.GetHashCode())
                    continue;
                this._childrenList.Add(temp);
            }

            this._attackingSprite = Resources.Load<Sprite>("Player/Sprites/AttackBanner"); 
            this._defendingSprite = Resources.Load<Sprite>("Player/Sprites/DefendBanner");
            this._endSprite = Resources.Load<Sprite>("Player/Sprites/EndBanner");
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

            this._banner.gameObject.SetActive(false);

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
            if(!this._banner.activeSelf)
                this._banner.SetActive(true);

            if(this.controller.isAttacking)
                this._bannerImage.sprite = this._attackingSprite;
            else
                this._bannerImage.sprite = this._defendingSprite;

            this._bannerImage.color = new Vector4(this._bannerImage.color.r, this._bannerImage.color.g, this._bannerImage.color.b, 0.0f);

            this._bannerTimer = this._bannerLimit;
            this._bannerFadeInTimer = this._fadeInLImit;
            this._bannerFadeOutTimer = this._fadeOutLmit;

            StartCoroutine(StartBannerAniamtion());
        }

        public IEnumerator StartBannerAniamtion() {

            if(this.controller.state != PlayerState.START)
                yield break;

            float bannerAlpha = 0.0f;

            while(this._bannerFadeInTimer >= 0.0f) { // FAde In

                this._bannerFadeInTimer -= Time.deltaTime;

                //Debug.Log("Banner Alpha Fade In Value: " + (1.0 - (this._bannerFadeInTimer * 0.5f)).ToString());

                bannerAlpha = (1.0f - (this._bannerFadeInTimer * 0.5f));
                this._bannerImage.color = new Vector4(1.0f, 1.0f, 1.0f, bannerAlpha);

                if(this._bannerFadeInTimer <= 0.0f) {
                    bannerAlpha = 1.0f;
                    this._bannerImage.color = new Vector4(1.0f, 1.0f, 1.0f, bannerAlpha);
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            bannerAlpha = 1.0f;
            this._bannerImage.color = new Vector4(1.0f, 1.0f, 1.0f, bannerAlpha);

            while(this._bannerTimer >= 0.0f) { // Static

                this._bannerTimer -= Time.deltaTime;

                if(this._bannerTimer <= 0.0f)
                    break;

                yield return new WaitForEndOfFrame();
            }

            while(this._bannerFadeOutTimer >= 0.0f) { // Fade Out

                this._bannerFadeOutTimer -= Time.deltaTime;

                //Debug.Log("Banner Alpha Fade Out Value: " + (this._bannerFadeOutTimer * 0.5f).ToString());

                bannerAlpha = (this._bannerFadeOutTimer * 0.5f);
                this._bannerImage.color = new Vector4(1.0f, 1.0f, 1.0f, bannerAlpha);
                    
                if(this._bannerFadeOutTimer <= 0.0f) {
                    bannerAlpha = 0.0f;
                    this._bannerImage.color = new Vector4(1.0f, 1.0f, 1.0f, bannerAlpha);
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            bannerAlpha = 0.0f;
            this._bannerImage.color = new Vector4(1.0f, 1.0f, 1.0f, bannerAlpha);

            this.controller.StartTurn();
            //this._banner.SetActive(false);

            yield return null;
        }

        public void FinishedBannerAnimation() {
            Debug.Log("Banner end Trigger");
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