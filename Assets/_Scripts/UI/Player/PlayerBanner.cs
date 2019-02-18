namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    [System.Serializable]
    public class PlayerBanner : MonoBehaviour {

        #region VARIABLE

        private bool _isAttacking = true;

        private float _bannerLinger = 1.0f;

        private PlayerUI _playerUI;

        [SerializeField] private GameObject _banner = null;
        [SerializeField] private GameObject _textBanner = null;
        [SerializeField] private GameObject _ribbonBanner = null;

        [SerializeField] private Sprite _attackBannerSprite = null;
        [SerializeField] private Sprite _attackTextBannerSprite = null;
        [SerializeField] private Sprite _defendBannerSprite = null;
        [SerializeField] private Sprite _defendTextBannerSprite = null;

        private Image _bannerImage = null;
        private Image _textBannerImage = null;

        private Animation _bannerAnim = null;
        private Animation _textBannerAnim = null;
        private Animation _ribbonAnim = null;

        #endregion

        #region CLASS

        public void Init(PlayerUI playerUI) {
            if(this._banner != null) {
                this._bannerImage = this._banner.GetComponent<Image>() as Image;
                this._bannerAnim = this._banner.GetComponent<Animation>() as Animation;
            }

            if(this._textBanner != null) {
                this._textBannerImage = this._textBanner.GetComponent<Image>() as Image;
                this._textBannerAnim = this._textBanner.GetComponent<Animation>() as Animation;
            }

            if(this._ribbonBanner != null) {
                this._ribbonAnim = this._ribbonBanner.GetComponent<Animation>() as Animation;
            }

            this._bannerImage.sprite = this._attackBannerSprite;
            this._textBannerImage.sprite = this._attackTextBannerSprite;

            this._playerUI = playerUI;
        }

        public void SwapBanner() {
            if(this._isAttacking) {
                this._isAttacking = false;
                this._bannerImage.sprite = this._defendBannerSprite;
                this._textBannerImage.sprite = this._defendTextBannerSprite;
            } else {
                this._isAttacking = true;
                this._bannerImage.sprite = this._attackBannerSprite;
                this._textBannerImage.sprite = this._attackTextBannerSprite;
            }
        }

        public void SwapBanner(bool attacking) {
            if(attacking) {
                this._isAttacking = true;
                this._bannerImage.sprite = this._attackBannerSprite;
                this._textBannerImage.sprite = this._attackTextBannerSprite;
            } else {
                this._isAttacking = false;
                this._bannerImage.sprite = this._defendBannerSprite;
                this._textBannerImage.sprite = this._defendTextBannerSprite;
            }
        }

        public void StartBannerAnimation() {

            this._bannerAnim.Play("BannerSpawn");
            this._ribbonAnim.Play("RibbonFadeIn");
            this._textBannerAnim.Play("PhaseTextFadeIn");

            float spawnAnimTimer = this._textBannerAnim.GetClip("PhaseTextFadeIn").length;

            Invoke("FinishBannerAnimation", spawnAnimTimer + this._bannerLinger);
        }

        public bool TurnOff() {
            if(!this.gameObject.activeSelf)
                return false;

            this.gameObject.SetActive(false);
            return true;
        }

        public bool TurnOn() {
            if(this.gameObject.activeSelf)
                return false;

            this.gameObject.SetActive(false);
            return true;
        }

        private void FinishBannerAnimation() {

            this._bannerAnim.Play("TurnBannerFadeOut");
            this._ribbonAnim.Play("TurnBannerFadeOut");
            this._textBannerAnim.Play("TurnBannerFadeOut");

            float endAnimationTimer = this._bannerAnim.GetClip("TurnBannerFadeOut").length;

            Invoke("FinishBanner", endAnimationTimer);
        }

        private void FinishBanner() {
            this._playerUI.FinishedBannerAnim();
        }

        #endregion
    }
}