﻿namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using TMPro;

    using Enum;
    using Manager;
    using Player;
    using System;

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
        public GameObject unitUIGroup = null;
        public GameObject structureUIGroup = null;

        [SerializeField] protected List<Transform> _uiChildrenList = new List<Transform>();

        #endregion

        #region UNITY

        public void Update() {

            if(this.Controller.CurrentState == PlayerState.ATTACKING || this.Controller.CurrentState == PlayerState.DEFENDING) {
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

            if(this.unitUIGroup == null) {
                this.unitUIGroup = this._uiGroup.transform.Find("Unit_UI").gameObject;
                this.unitUIGroup.SetActive(false);
            }

            if(this.structureUIGroup == null) {
                this.structureUIGroup = this._uiGroup.transform.Find("Structure_UI").gameObject;
                this.structureUIGroup.SetActive(false);
            }

            foreach(Transform temp in this._uiGroup.GetComponentInChildren<Transform>()) {
                if(temp.GetHashCode() == this.bannerGroup.transform.GetHashCode())
                    continue;

                if(temp.name == "Spawn")
                    this.spawnGroup = temp.gameObject;

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

                if(temp.gameObject == this.unitUIGroup)
                    continue;

                if(temp.gameObject == this.structureUIGroup)
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

        public override void OnEnter() {
            throw new NotImplementedException();
        }

        public override void OnExit() {
            throw new NotImplementedException();
        }

        public override void ResetUI() {
            throw new System.NotImplementedException();
        }

        public override void UpdateUI() {
            if(this.Controller.TurnEnded) {
                this.HideUI();
            } else {
                this.UpdateInfo();
            }
        }

        public void ShowBanner() {
            if(!this.bannerGroup.activeSelf)
                this.bannerGroup.SetActive(true);

            this._playerBanner.SwapBanner(this.Controller.IsAttacking);

            this._playerBanner.StartBannerAnimation();
        }

        public void FinishedBannerAnim() {
            this.Controller.StartTurn();
        }

        public void EndTurn() {
            this.HideUI();
            this.Controller.EndTurn();
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