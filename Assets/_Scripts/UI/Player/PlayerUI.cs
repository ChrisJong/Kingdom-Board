namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using TMPro;

    using Constants;
    using Enum;
    using Manager;
    using Player;
    using Utility;

    public class PlayerUI : UIBase {

        #region VARIABLE

        [Space]
        [SerializeField] private PlayerEndButton _endTurnButton = null;
        [SerializeField] private PlayerBanner _playerBanner = null;
        [SerializeField] private PlayerHourglass _playerHourglass = null;

        [SerializeField] private LineRenderDrawCircle _radiusDrawer = null; 

        [Space]
        [SerializeField] private GameObject _persistantGroup = null;
        private RectTransform _persistantTransform = null;

        [SerializeField] private TextMeshProUGUI _infoText;

        [Space]
        public GameObject bannerGroup = null;
        public GameObject spawnGroup = null;
        public GameObject researchGroup = null;
        public GameObject unitUIGroup = null;
        public GameObject unitLocator = null;
        public GameObject structureUIGroup = null;

        [Space]
        [SerializeField] protected List<Transform> _uiChildrenList = new List<Transform>();

        public LineRenderDrawCircle RadiusDrawer { get { return this._radiusDrawer; } }

        #endregion

        #region UNITY

        public void Update() {

            if(this._controller.CurrentState == PlayerState.ATTACKING || this._controller.CurrentState == PlayerState.DEFENDING || this._controller.CurrentState == PlayerState.WAITING) {
                this._endTurnButton.UpdateButton();
                this.UpdateInfo();
            }
        }

        #endregion

        #region CLASS
        public override void Setup() {
            base.Setup();

            if(this.unitUIGroup == null) {
                this.unitUIGroup = this._mainGroup.transform.Find(UIValues.Unit.UNIT_UI_MAIN).gameObject;
                this.unitUIGroup.SetActive(false);
            }

            if(this.structureUIGroup == null) {
                this.structureUIGroup = this._mainGroup.transform.Find(UIValues.Structure.STRUCTURE_UI_MAIN).gameObject;
                this.structureUIGroup.SetActive(false);
            }

            if(this._persistantGroup == null)
                this._persistantGroup = this._mainRectTransform.Find(UIValues.PERSISTANT_GROUP).gameObject;
            this._persistantTransform = this._persistantGroup.transform as RectTransform;

            if(this._infoText == null)
                this._infoText = this._persistantTransform.Find("info_TEXT").GetComponent<TextMeshProUGUI>() as TextMeshProUGUI;

            if(this._endTurnButton == null) {
                this._endTurnButton = this._persistantTransform.Find("End_BTN").GetComponent<PlayerEndButton>() as PlayerEndButton;
            }

            if(this.bannerGroup == null) {
                this.bannerGroup = this._mainGroup.transform.Find("Banner").gameObject;
                this._playerBanner = this.bannerGroup.GetComponent<PlayerBanner>() as PlayerBanner;
            } else if(this._playerBanner == null) {
                this._playerBanner = this.bannerGroup.GetComponent<PlayerBanner>() as PlayerBanner;
            }
        }

        public override void Init(Player controller) {
            base.Init(controller);

            this._endTurnButton.Init(this);
            this._playerBanner.Init(this);

            if(this._playerHourglass == null) {
                this._playerHourglass = this._controller.playerCamera.MainCamera.GetComponentInChildren<PlayerHourglass>();
            }
            this._playerHourglass.Setup(this._controller);

            this._uiChildrenList.Add(this._playerHourglass.transform);

            foreach(Transform temp in this._mainGroup.GetComponentInChildren<Transform>()) {
                if(temp.GetHashCode() == this.bannerGroup.transform.GetHashCode())
                    continue;

                if(temp.name == "Spawn")
                    this.spawnGroup = temp.gameObject;

                if(temp.name == "Research")
                    this.researchGroup = temp.gameObject;

                this._uiChildrenList.Add(temp);
            }

        }

        public override void Display() {
            foreach(Transform temp in this._uiChildrenList) {
                if(temp.gameObject.activeSelf)
                    continue;

                if(temp.gameObject == this.unitUIGroup)
                    continue;

                if(temp.gameObject == this.structureUIGroup)
                    continue;

                temp.gameObject.SetActive(true);
            }

            this.UpdateInfo();
        }

        public override void Hide() {
            this.bannerGroup.gameObject.SetActive(false);

            foreach(Transform temp in this._uiChildrenList) {
                if(!temp.gameObject.activeSelf)
                    continue;

                temp.gameObject.SetActive(false);
            }
        }

        public override void OnEnter() {
            throw new System.NotImplementedException();
        }

        public override void OnEnter(Player controller) {
            throw new System.NotImplementedException();
        }

        public override void OnExit() {
            throw new System.NotImplementedException();
        }

        public override void ResetUI() {
            throw new System.NotImplementedException();
        }

        public override void UpdateUI() {
            if(this.Controller.TurnEnded) {
                this.Hide();
            } else {
                this.UpdateInfo();
            }
        }

        public void Starthourglass() {
            this._playerHourglass.StartHourglass();
        }

        public void ShowBanner() {
            if(!this.bannerGroup.activeSelf)
                this.bannerGroup.SetActive(true);

            if(this.Controller.IsAttacking)
                this.Controller.playerSound.PlayAttackPhase();
            else
                this.Controller.playerSound.PlayDefencePhase();

            this._playerBanner.SwapBanner(this.Controller.IsAttacking);

            this._playerBanner.StartBannerAnimation();
        }

        public void FinishedBannerAnim() {
            this.Controller.StartTurn();
        }

        private void UpdateInfo() {
            string text = string.Empty;

            int gold = ResourceManager.instance.GetPlayerResource(this.Controller, PlayerResource.GOLD);
            int population = ResourceManager.instance.GetPlayerResource(this.Controller, PlayerResource.POPULATION);
            int populationCap = ResourceManager.instance.PopulationCap;

            text = "GOLD: " + gold.ToString() + "\r\n" +
                   "UNIT CAP: " + population.ToString() + " / " + populationCap.ToString() + "\r\n" + 
                   "PHASE: " + this.Controller.CurrentState.ToString();

            this._infoText.text = text;
        }
        #endregion
    }
}