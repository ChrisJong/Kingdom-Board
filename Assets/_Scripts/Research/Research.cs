﻿namespace Research {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Enum;
    using Manager;
    using Player;
    using Scriptable;
    using Structure;
    using Unit;

    public class Research : MonoBehaviour {

        #region VARIABLE

        [SerializeField] private Player _controller;
        [SerializeField] private Castle _castle;

        [SerializeField] private ResearchState _currentState = ResearchState.NONE;
        [SerializeField] private ResearchState _previousState = ResearchState.NONE;

        [SerializeField] private ClassType _classSelected = ClassType.NONE;
        [SerializeField] private ClassType _previousClassSelected = ClassType.NONE;
        [SerializeField] private UnitType _unitSelected = UnitType.NONE;

        [SerializeField] private int[] _researchTurns = { 1, 3, 6, 12, 36 };
        [SerializeField] private int _researchCount = 0;
        [SerializeField] private int _cardsReady = 0;

        [SerializeField] private bool _isUnitResearch = false;

        [Header("UI - VALUES/POSITION")]
        [SerializeField] private float _padding = 50.0f;
        [SerializeField] private float _cardWidth = 150.0f;
        [SerializeField] private float _cardHeight = 210.5f;

        [SerializeField] private ResearchCard _previousCardSelected = null;
        [SerializeField] private ResearchCard _currentCardSelected = null;

        [SerializeField] private List<ClassScriptable> _classDataList = new List<ClassScriptable>();
        [SerializeField] private List<UpgradeScriptable> _upgradeDataList = new List<UpgradeScriptable>(); // List that holds all the upgrades available.

        [SerializeField] private List<ResearchCard> _classCards = new List<ResearchCard>();
        private Dictionary<ClassType, ResearchUnitGroup> _unitCards = new Dictionary<ClassType, ResearchUnitGroup>();
        private Dictionary<ClassType, ResearchUpgradeGroup> _upgradeCards = new Dictionary<ClassType, ResearchUpgradeGroup>();
        private Dictionary<ClassType, List<ResearchUpgradeData>> _unitUprades = new Dictionary<ClassType, List<ResearchUpgradeData>>();
        [SerializeField] private List<ResearchCard> _cardsToDisplay = new List<ResearchCard>();

        [SerializeField] private Dictionary<ClassType, List<ResearchUpgradeData>> _upgrades = new Dictionary<ClassType, List<ResearchUpgradeData>>();

        [SerializeField] private GameObject _researchGroup = null;
        [SerializeField] private GameObject _prefabResearchCard = null;
        [SerializeField] private GameObject _prefabBackButton = null;

        private Button _backButton;

        public int ResearchCount { get { return this._researchCount; } }
        public int CardsReady { get { return this._cardsReady; } set { this._cardsReady = value; } }

        public ResearchState CurrentState { get { return this._currentState; } }
        public ResearchState PreviousState { get { return this._previousState; } }

        #endregion

        #region UNITY

        private void Update() {
            if(this._currentState != ResearchState.BACK)
                return;
            else {
                if(this._cardsReady == this._cardsToDisplay.Count)
                    this.Back();
            }
        }

        #endregion

        #region CLASS

        public void Init(Player controller) {
            this._controller = controller;
            this._castle = controller.castle;

            if(this._backButton == null) {
                GameObject temp = null;
                RectTransform rectTransform = null;
                Vector3 pos = new Vector3(0.0f, 0.0f - ((216 / 2 + 70 / 2) + 10.0f), 0.0f);

                if(this._prefabBackButton != null)
                    temp = Instantiate(this._prefabBackButton, this._researchGroup.transform);
                else
                    Debug.LogError("Back Button Prefab Cannot Be Empty!");

                rectTransform = temp.transform as RectTransform;
                rectTransform.anchoredPosition = pos;

                this._backButton = temp.GetComponent<Button>() as Button;
                this._backButton.onClick.AddListener(BackButton);
                this._backButton.gameObject.SetActive(false);
            }

            if(this.LoadClassData() && this.LoadUpgradeData()) {
                this.GenerateClassCards();
                this.GenerateUnitCards();
                this.GenerateUpgradeCards();
            }
        }

        public bool CheckResearchPhase(int currentTurn) {

            this._previousState = ResearchState.NONE;
            this._currentState = ResearchState.START;

            if(this.ResearchCount > this._researchTurns.Length) {
                Debug.Log("Current Turn: " + currentTurn.ToString() + " - " + " Upgrade Research");
                this._isUnitResearch = false;
                this.DisplayResearchCards();
                return false;
            }

            if(currentTurn == this._researchTurns[this._researchCount]) {
                Debug.Log("Current Turn: " + currentTurn.ToString() + " - " + " Unit Research");

                this._researchCount++;
                this._isUnitResearch = true;
                this.DisplayResearchCards();
                return true;
            }

            Debug.Log("Current Turn: " + currentTurn.ToString() + " - " + " Upgrade Research");

            this._isUnitResearch = false;
            this.DisplayResearchCards();
            return false;
        }

        public void SelectedCard(int keyID = -1, ClassType classType = ClassType.NONE, UnitType unitType = UnitType.NONE, UnitUpgradeType upgradeType = UnitUpgradeType.NONE) {
            if(this._currentState == ResearchState.CLASS) {

                foreach(ResearchCard classCard in this._classCards) {
                    if(classCard.ClassType == classType) {
                        this._currentCardSelected = classCard;
                    } else {
                        classCard.CardAnimation.PlayFadeAnimation();
                    }
                }

                this._classSelected = classType;
                this._unitSelected = unitType;

                if(this._isUnitResearch) {
                    this._previousState = this._currentState;
                    this._currentState = ResearchState.UNIT;
                } else {
                    this._previousState = this._currentState;
                    this._currentState = ResearchState.UPGRADE;
                }

                this._backButton.gameObject.SetActive(true);
                this.DisplayResearchCards();

            } else if(this._currentState == ResearchState.UNIT) {

                this._castle.UnlockUnitToSpawn(classType, unitType);

                for(int i = 0; i < this._cardsToDisplay.Count; i++) {
                    if(this._cardsToDisplay[i].UnitType != unitType)
                        StartCoroutine(this._cardsToDisplay[i].CardAnimation.RotateAndFade());
                    else {
                        this._previousCardSelected = this._currentCardSelected;
                        this._currentCardSelected = this._cardsToDisplay[i];
                    }
                }

                this._backButton.gameObject.SetActive(false);

                this._previousClassSelected = this._classSelected;
                this._classSelected = classType;
                this._unitSelected = unitType;

                this._previousState = this._currentState;
                this._currentState = ResearchState.FINISHED;

            } else if(this._currentState == ResearchState.UPGRADE) {
                ResearchUpgradeData data = this._upgrades[classType][keyID];

                // Change the upgrade data in the research class.
                data.ResearchUpgrade();

                // Update the unit data in the controller class.
                if(this._controller.ClassUnits[classType].Count != 0) {
                    foreach(UnitBase unit in this._controller.ClassUnits[classType]) {
                        this.ApplyUpgrade(unit, data);
                    }
                }

                foreach(ResearchCard card in this._cardsToDisplay) {
                    if(card.UpgradeType != upgradeType) {
                        StartCoroutine(card.CardAnimation.RotateAndFade());
                    } else {
                        this._previousCardSelected = this._currentCardSelected;
                        this._currentCardSelected = card;
                    }
                }

                this._backButton.gameObject.SetActive(false);

                this._previousState = this._currentState;
                this._currentState = ResearchState.FINISHED;
            }
        }

        public void Back() {
            //this._previousState = this._currentState;
            this._currentState = ResearchState.CLASS;
            this._backButton.gameObject.SetActive(false);
            this._cardsReady = 0;

            Debug.Log("Back calls");

            foreach(ResearchCard card in this._cardsToDisplay) {
                card.CardAnimation.PlayFadeAnimation();
            }

            this.DisplayResearchCards();
        }

        public void ApplyUpgrades(IUnit unit) {

            foreach(ResearchUpgradeData data in this._upgrades[unit.classType]) {

                UnitBase temp = unit as UnitBase;

                if(data.ResearchCount == 0)
                    continue;
                else
                    temp.ApplyUpgrade(data.UpgradeType, data.CurrentValue);
            }
        }

        private void ApplyUpgrade(UnitBase Unit, ResearchUpgradeData data) {
            if(data.ResearchCount == 0)
                return;

            Unit.ApplyUpgrade(data.UpgradeType, data.CurrentValue);
        }

        private void DisplayResearchCards() {

            //this._cardsToDisplay.Clear();
            //this._cardsToDisplay = null

            if(this._currentState == ResearchState.UNIT) {
                this.DisplayUnitCards();
            } else if(this._currentState == ResearchState.UPGRADE) {
                this.DisplayUpgradeCards();
            } else {
                this.DisplayClassCards();
            }
        }

        private void DisplayClassCards() {
            if(this._currentState == ResearchState.START) {

                List<ResearchCard> temp = new List<ResearchCard>();

                foreach(ResearchCard classCard in this._classCards) {

                    if(this._isUnitResearch) { // Unit Research
                        if(classCard.ClassType == this._previousClassSelected)
                            continue;
                        if(this._unitCards[classCard.ClassType].AnyCardsToUnlock()) {
                            classCard.DisplayCard();
                            temp.Add(classCard);
                        }
                    } else { // Upgrade Research
                        if(!this._unitCards[classCard.ClassType].IsBaseTierUnlocked()) {
                            continue;
                        } else {
                            classCard.DisplayCard();
                            temp.Add(classCard);
                        }
                    }
                }

                this._cardsToDisplay = temp;
                this.RepositionCards();

                this._previousState = this._currentState;
                this._currentState = ResearchState.CLASS;

                foreach(ResearchCard card in this._cardsToDisplay) {
                    card.ResetCard();
                    card.CardAnimation.PlaySpawnAnimation();
                }

            } else {

                List<ResearchCard> temp = new List<ResearchCard>();

                foreach(ResearchCard classCard in this._classCards) {

                    if(this._isUnitResearch) { // Unit Research
                        if(classCard.ClassType == this._previousClassSelected)
                            continue;
                        if(this._unitCards[classCard.ClassType].AnyCardsToUnlock()) {
                            classCard.DisplayCard();
                            temp.Add(classCard);
                        }
                    } else { // Upgrade Researchj
                        if(!this._unitCards[classCard.ClassType].IsBaseTierUnlocked()) {
                            continue;
                        } else {
                            classCard.DisplayCard();
                            temp.Add(classCard);
                        }
                    }
                }

                this._cardsToDisplay = temp;

                this._previousState = this._currentState;
                this._currentState = ResearchState.CLASS;

                foreach(ResearchClassCard card in this._cardsToDisplay) {
                    card.SetPosition(Vector3.zero);
                }

                this.SetMovePositions();
            }
        }

        private void DisplayUnitCards() {
            ResearchUnitGroup tierGroup = this._unitCards[this._classSelected];
            List<ResearchCard> temp = new List<ResearchCard>();

            foreach(ResearchUnitCard card in tierGroup.GetCards()) {
                //this._cardsToDisplay.Add(card);
                temp.Add(((ResearchCard)card));
            }

            //this._cardsToDisplay.Clear();
            //this._cardsToDisplay = null;
            this._cardsToDisplay = temp;

            if(this._cardsToDisplay.Count != 0) {
                foreach(ResearchCard card in this._cardsToDisplay) {
                    card.DisplayCard();
                    card.SetPosition(this._currentCardSelected.rectTransform.anchoredPosition);
                }

                this.SetMovePositions();

            } else {
                Debug.LogWarning("THERE ARE NO CARDS TO DISPLAY: " + this._cardsToDisplay.Count.ToString());
            }
        }

        private void DisplayUpgradeCards() {
            ResearchUpgradeGroup group = this._upgradeCards[this._classSelected];
            List<ResearchCard> temp = new List<ResearchCard>();

            foreach(ResearchUpgradeCard card in group.GetCards()) {
                //this._cardsToDisplay.Add(card);
                temp.Add(((ResearchCard)card));
                card.UpdateCard();
            }

            this._cardsToDisplay = temp;

            foreach(ResearchCard card in this._cardsToDisplay) {
                card.DisplayCard();
                card.SetPosition(this._currentCardSelected.rectTransform.anchoredPosition);
            }

            this.SetMovePositions();
        }

        private void HideCards() {
            if(this._currentState == ResearchState.CLASS) {
                foreach(ResearchCard card in this._classCards) {
                    if(card.Toggled)
                        card.HideCard();
                    else
                        continue;
                }
            } else {
                foreach(ResearchUnitCard card in this._cardsToDisplay) {
                    if(card.Toggled)
                        card.HideCard();
                    else
                        continue;
                }
            }
        }

        private void BackButton() {
            if(this._currentState == ResearchState.UNIT) {

                foreach(ResearchUnitCard card in this._cardsToDisplay) {
                    card.CardAnimation.Back(Vector3.zero);
                }

                this._previousState = this._currentState;
                this._currentState = ResearchState.BACK;

            } else if(this._currentState == ResearchState.UPGRADE) {

                
                foreach(ResearchUpgradeCard card in this._cardsToDisplay) {
                    card.CardAnimation.Back(Vector3.zero);
                }

                this._previousState = this._currentState;
                this._currentState = ResearchState.BACK;
            }
        }

        private void RepositionCards() {
            float startPos = -200.0f; // For 3 cards.

            for(int i = 0; i < this._cardsToDisplay.Count; i++) {
                if(this._cardsToDisplay.Count == 1) {
                    this._cardsToDisplay[i].SetPosition(Vector3.zero);
                } else if(this._cardsToDisplay.Count == 2) {
                    if(i == 0) // Left
                        this._cardsToDisplay[i].SetPosition(new Vector3(-100.0f, 0.0f, 0.0f));
                    else // Right
                        this._cardsToDisplay[i].SetPosition(new Vector3(100.0f, 0.0f, 0.0f));
                } else if(this._cardsToDisplay.Count > 2) {
                    this._cardsToDisplay[i].SetPosition(new Vector3(startPos, 0.0f, 0.0f));
                    startPos += 200.0f;
                }
            }
        }

        private void SetMovePositions() {
            float startPos = -200.0f; // For 3 cards.

            for(int i = 0; i < this._cardsToDisplay.Count; i++) {
                if(this._cardsToDisplay.Count == 1) {
                    this._cardsToDisplay[i].CardAnimation.SetMoveTo(Vector3.zero);
                } else if(this._cardsToDisplay.Count == 2) {
                    if(i == 0) // Left
                        this._cardsToDisplay[i].CardAnimation.SetMoveTo(new Vector3(-100.0f, 0.0f, 0.0f));
                    else // Right
                        this._cardsToDisplay[i].CardAnimation.SetMoveTo(new Vector3(100.0f, 0.0f, 0.0f));
                } else if(this._cardsToDisplay.Count > 2) {
                    Debug.Log("Card: " + this._cardsToDisplay[i].ClassType.ToString());
                    this._cardsToDisplay[i].CardAnimation.SetMoveTo(new Vector3(startPos, 0.0f, 0.0f));
                    startPos += 200.0f;
                }
            }
        }

        private bool LoadClassData() {
            Object[] temp = Resources.LoadAll("Class", typeof(ClassScriptable));

            foreach(Object data in temp) {
                this._classDataList.Add((ClassScriptable)data);
            }

            if(this._classDataList.Count > 0)
                return true;
            else
                return false;
        }

        private bool LoadUpgradeData() {

            int classCount = System.Enum.GetNames(typeof(ClassType)).Length - 2;

            for(int i = 0; i < classCount; i++) {
                string className = ((ClassType)i + 1).ToString();
                string path = "Upgrades/" + className;
                Object[] temp = Resources.LoadAll(path, typeof(UpgradeScriptable));

                foreach(Object data in temp)
                    this._upgradeDataList.Add(data as UpgradeScriptable);
            }

            if(this._upgradeDataList.Count > 0)
                return true;
            else
                return false;
        }

        private void GenerateClassCards() {
            float startpos = -200.0f; // this value should change with the size of the screen.
            int classCount = System.Enum.GetNames(typeof(ClassType)).Length - 2;
            
            for(int i = 0; i < classCount; i++) {
                ClassScriptable data = this._classDataList[i];
                GameObject go = Instantiate(this._prefabResearchCard, this._researchGroup.transform);;
                ResearchClassCard card = go.AddComponent<ResearchClassCard>() as ResearchClassCard;
                Vector3 pos = new Vector3(startpos, 0.0f, 1.0f);

                startpos += 200.0f;

                go.name = data.classType.ToString() + "_CARD";

                // NOTE: Find the face and back sprite here and replace the "Nulls" in the init function below.

                card.Init(this, data.cardFaceSprite, data.cardBackSprite, pos, data.classType, UnitType.NONE, i);

                this._classCards.Add(card as ResearchCard);

                card.HideCard();
            }
        }

        private void GenerateUnitCards() {
            int classCount = System.Enum.GetNames(typeof(ClassType)).Length - 2;
            int unitCount = System.Enum.GetNames(typeof(UnitType)).Length - 2;

            for(int i = 0; i < classCount; i++) {
                List<ResearchUnitCard> container = new List<ResearchUnitCard>();
                ClassType type = (ClassType)i + 1;

                for(int j = 0; j < unitCount; j++) {
                    UnitScriptable unitData = UnitPoolManager.instance.UnitDataList[j];

                    if(unitData.classType == type) {
                        GameObject go = Instantiate(this._prefabResearchCard, this._researchGroup.transform);
                        ResearchUnitCard card = go.AddComponent<ResearchUnitCard>() as ResearchUnitCard;

                        go.name = unitData.unitType.ToString() + "_CARD";

                        card.Init(this, unitData, Vector3.zero);
                        container.Add(card);

                        card.HideCard();
                    }
                }

                ResearchUnitGroup tierGroup;

                if(container.Count != 0)
                    tierGroup = new ResearchUnitGroup(container, 1);
                else
                    tierGroup = new ResearchUnitGroup(container);

                this._unitCards.Add(type, tierGroup);
            }
        }

        private void GenerateUpgradeCards() {

            int classCount = System.Enum.GetNames(typeof(ClassType)).Length - 2;
            int upgradecount = this._upgradeDataList.Count;

            for(int i = 0; i < classCount; i++) {

                List<ResearchUpgradeCard> container = new List<ResearchUpgradeCard>();
                List<ResearchUpgradeData> dataList = new List<ResearchUpgradeData>();
                ClassType type = (ClassType)i + 1;

                int count = 0;

                for(int j = 0; j < upgradecount; j++) {

                    UpgradeScriptable data = null;
                    ResearchUpgradeData upgradeData = null;

                    if(upgradecount != 0)
                        data = this._upgradeDataList[j];
                    else
                        Debug.LogError("Upgrade DAta Is Empty: " + upgradecount.ToString());

                    if(data.classType == type) {
                        GameObject go = Instantiate(this._prefabResearchCard, this._researchGroup.transform);
                        ResearchUpgradeCard card = go.AddComponent<ResearchUpgradeCard>() as ResearchUpgradeCard;

                        upgradeData = new ResearchUpgradeData(count, data);
                        go.name = data.classType.ToString() + "_" + data.upgradeType.ToString() + "_CARD";

                        card.Init(this, data, upgradeData, Vector3.zero, count);
                        container.Add(card);
                        dataList.Add(upgradeData);

                        card.HideCard();
                        count++;
                    }
                }

                ResearchUpgradeGroup group;

                if(container.Count != 0)
                    group = new ResearchUpgradeGroup(type, container);
                else
                    group = null;

                this._upgradeCards.Add(type, group);
                this._upgrades.Add(type, dataList);
            }
        }

        #endregion
    }
}