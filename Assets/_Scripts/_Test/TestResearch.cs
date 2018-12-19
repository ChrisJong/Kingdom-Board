namespace Test {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Enum;
    using Manager;

    public class TestResearch : MonoBehaviour {

        public enum States {
            NONE = 0,
            ANY = ~0,
            CLASS = 1,
            RESEARCH_CARDS = 2,
            FINISHED,
        }

        public enum ResearchState {
            NONE = 0,
            ANY = ~0,
            CLASS = 1,
            UNITS = 2,
            UPGRADE = 3,
            FINISHED
        };

        [SerializeField] private ResearchState _state = ResearchState.NONE;
        [SerializeField] private ResearchState _researchState = ResearchState.NONE;
        [SerializeField] private bool _isUnitResearch = false;

        public ResearchState State {
            get { return this._state; }
        }

        [SerializeField] private GameObject _researchGroup;
        [SerializeField] private GameObject _prefabResearchCard;
        [SerializeField] private GameObject _prefavResearchUpgradeCard;
        [SerializeField] private GameObject _prefabBackButton;

        [SerializeField] private Button _backButton;

        [SerializeField] private int _researchCount = 0;
        public int ResearchCount {
            get { return this._researchCount; }
            set { this._researchCount = value; }
        }

        [SerializeField] private int[] _researchTurns = { 1, 3, 6, 12, 36 };

        [SerializeField] private TestCastle _castle;

        [SerializeField] private List<TestUpgradeScriptable> _upgradeStructure = new List<TestUpgradeScriptable>();
        [SerializeField] private TestResearchCard[] _classCards = new TestResearchCard[3];
        private Dictionary<ClassType, TestResearachTierGroup> _unitCards;
        private Dictionary<ClassType, TestResearchUpgradeGroup> _upgradeCards;
        private List<TestResearchCard> _cardsToDisplay;
        private List<TestResearchUpgradeCard> _upgradeCardsToDisplay = new List<TestResearchUpgradeCard>();

        [SerializeField] private ClassType _classSelected = ClassType.NONE;
        [SerializeField] private ClassType _preivousClassSelected = ClassType.NONE;
        [SerializeField] private UnitType _unitSelected = UnitType.NONE;

        [SerializeField] private float _padding = 50.0f;
        [SerializeField] private float _cardWidth = 150.0f;
        [SerializeField] private float _cardHeight = 216.5f;

        private void Awake() {
            if(this._castle == null)    
                this._castle = this.transform.GetComponent<TestCastle>() as TestCastle;

            this.Init();
        }

        public void Init() { 
            this._unitCards = new Dictionary<ClassType, TestResearachTierGroup>();
            this._upgradeCards = new Dictionary<ClassType, TestResearchUpgradeGroup>();
            this._cardsToDisplay = new List<TestResearchCard>();
            //this._upgradeCardsToDisplay = new List<TestResearchUpgradeCard>();

            if(this._backButton == null) {
                GameObject temp = null;
                RectTransform rectTransform = null;
                Vector3 pos = new Vector3(0.0f, 0.0f - ((216/2 + 70/2) + 10.0f), 0.0f);

                if(this._prefabBackButton != null)
                    temp = Instantiate(this._prefabBackButton, this._researchGroup.transform);
                else
                    Debug.LogError("Back Button Prefab Cannot Be Empty!");

                rectTransform = temp.transform as RectTransform;
                rectTransform.anchoredPosition = pos;

                this._backButton = temp.GetComponent<Button>() as Button;
                this._backButton.onClick.AddListener(Back);
                this._backButton.gameObject.SetActive(false);
            }

            this.GenerateClassCards();
            this.GenerateUnitCards();
            this.GenerateUpgradeCards();

            //this.DebugUnitList();
        }

        private void GenerateClassCards() {
            float startpos = -200.0f;

            for(int i = 0; i < 3; i++) {
                GameObject go = Instantiate(this._prefabResearchCard, this._researchGroup.transform);
                TestResearchCard card = go.GetComponent<TestResearchCard>() as TestResearchCard;
                ClassType type = (ClassType)i+1;
                Vector3 pos = new Vector3(startpos, 0.0f, 1.0f);

                startpos += 200.0f;

                go.name = type.ToString() + "_CARD";

                card.Init(this, pos, type);

                this._classCards[i] = card;

                card.HideCard();
            }
        }

        public void GenerateUnitCards() {

            int classCount = System.Enum.GetNames(typeof(ClassType)).Length - 2;
            int unitCount = System.Enum.GetNames(typeof(UnitType)).Length - 2;

            for(int i = 0; i < classCount; i++) {
                List<TestResearchCard> container = new List<TestResearchCard>();
                ClassType type = (ClassType)i + 1;
                
                for(int j = 0; j < unitCount; j++) {
                    TestUnitScriptable unitData = null;// = UnitPoolManager.instance.UnitStructure[j];

                    if(unitData.classType == type) {
                        GameObject go = Instantiate(this._prefabResearchCard, this._researchGroup.transform);
                        TestResearchCard card = go.GetComponent<TestResearchCard>() as TestResearchCard;

                        go.name = unitData.unitType.ToString() + "_CARD";

                        card.Init(this, unitData, Vector3.zero, unitData.classType, unitData.unitType, unitData.tierLevel);
                        container.Add(card);

                        card.HideCard();
                    }
                }

                TestResearachTierGroup tierGroup;

                if(container.Count != 0)
                    tierGroup = new TestResearachTierGroup(container, 1);
                else
                    tierGroup = new TestResearachTierGroup(container);

                this._unitCards.Add(type, tierGroup);

            }
        }

        public void GenerateUpgradeCards() {
            int classCount = System.Enum.GetNames(typeof(ClassType)).Length - 2;
            int upgradecount = this._upgradeStructure.Count;

            for(int i = 0; i < classCount; i++) {
                List<TestResearchUpgradeCard> container = new List<TestResearchUpgradeCard>();
                ClassType type = (ClassType)i + 1;

                for(int j = 0; j < upgradecount; j++) {
                    TestUpgradeScriptable upgradeData = null;
                    if(this._upgradeStructure.Count != 0)
                        upgradeData = this._upgradeStructure[j];
                    else
                        Debug.LogError("Upgrade DAta Is Empty: " + this._upgradeStructure.Count.ToString());
                    
                    if(upgradeData.classType == type) {
                        GameObject go = Instantiate(this._prefavResearchUpgradeCard, this._researchGroup.transform);
                        TestResearchUpgradeCard card = go.GetComponent<TestResearchUpgradeCard>() as TestResearchUpgradeCard;

                        go.name = upgradeData.classType.ToString() + "_" + upgradeData.upgrade.ToString() + "_CARD";

                        card.Init(this, upgradeData, Vector3.zero, j);
                        container.Add(card);

                        card.HideCard();
                    }
                }

                TestResearchUpgradeGroup group;

                Debug.Log("Container Type: " + type.ToString() + " - " + container.Count.ToString());

                if(container.Count != 0)
                    group = new TestResearchUpgradeGroup(type, container);
                else
                    group = null;

                this._upgradeCards.Add(type, group);
            }
        }

        public void DisplayResearchCards() {

            //this._cardsToDisplay.Clear();
            this._cardsToDisplay = null;

            //if(this._upgradeCardsToDisplay.Count >= 1)
                //this._upgradeCardsToDisplay.Clear();
            this._upgradeCardsToDisplay = null;

            if(this._state == ResearchState.CLASS) {

                List<TestResearchCard> temp = new List<TestResearchCard>();

                foreach(TestResearchCard classCard in this._classCards) {

                    if(this._isUnitResearch) { // Unit Research
                        if(classCard.ClassType == this._preivousClassSelected)
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
                this.RepositionCards();

            } else if(this._state == ResearchState.UNITS) {
                this.DisplayUnitCards();
            } else if(this._state == ResearchState.UPGRADE) {
                this.DisplayUpgradeCards();
            }
        }

        public void DisplayUnitCards() {
            // Returns a list of cards and displays them on screen.
            TestResearachTierGroup tierGroup = this._unitCards[this._classSelected];

            this._cardsToDisplay = tierGroup.GetLockedCards();

            if(this._cardsToDisplay.Count != 0) {
                foreach(TestResearchCard card in this._cardsToDisplay) {
                    card.DisplayCard();
                    this.RepositionCards();
                }
            } else {
                Debug.LogWarning("THERE ARE NO CARDS TO DISPLAY: " + this._cardsToDisplay.Count.ToString());
            }
        }

        public void DisplayUpgradeCards() {
            TestResearchUpgradeGroup group = this._upgradeCards[this._classSelected];

            this._upgradeCardsToDisplay = group.GetUpgradeCards();

            foreach(TestResearchUpgradeCard card in this._upgradeCardsToDisplay) {
                card.DisplayCard();
                this.RepositionUpgradeCards();
            }
        }

        public void HideCards() {
            if(this._state == ResearchState.CLASS) {
                foreach(TestResearchCard card in this._classCards) {
                    if(card.Toggled)
                        card.HideCard();
                    else
                        continue;
                }
            } else if(this._state == ResearchState.UNITS) {
                foreach(TestResearchCard card in this._cardsToDisplay) {
                    if(card.Toggled)
                        card.HideCard();
                    else
                        continue;
                }

            } else if(this._state == ResearchState.UPGRADE) {
                foreach(TestResearchUpgradeCard card in this._upgradeCardsToDisplay) {
                    if(card.Toggled)
                        card.HideCard();
                    else
                        continue;
                }
            } else {

            }
        }

        private void Back() {
            if(this._state == ResearchState.UNITS) {

                this.HideCards();
                this._state = ResearchState.CLASS;
                this._backButton.gameObject.SetActive(false);

                this.DisplayResearchCards();

            } else if(this._state == ResearchState.UPGRADE) {

                this.HideCards();
                this._state = ResearchState.CLASS;
                this._backButton.gameObject.SetActive(false);

                this.DisplayResearchCards();
            }
        } 

        public void SelectedCard(ClassType classType = ClassType.NONE, UnitType unitType = UnitType.NONE, int keyID = -1) {
            if(this._state == ResearchState.CLASS) {

                foreach(TestResearchCard classCard in this._classCards) {
                    classCard.HideCard();
                }

                this._classSelected = classType;
                this._unitSelected = unitType;

                if(this._isUnitResearch)
                    this._state = ResearchState.UNITS;
                else
                    this._state = ResearchState.UPGRADE;

                this._backButton.gameObject.SetActive(true);
                this.DisplayResearchCards();

            } else if(this._state == ResearchState.UNITS) {

                this._castle.UnlockUnitSpawn(classType, unitType);

                for(int i = 0; i < this._cardsToDisplay.Count; i++)
                    this._cardsToDisplay[i].HideCard();

                this._backButton.gameObject.SetActive(false);

                this._preivousClassSelected = this._classSelected;
                this._classSelected = classType;
                this._unitSelected = unitType;

                this._state = ResearchState.FINISHED;

            } else if(this._state == ResearchState.UPGRADE) {
                TestUpgradeScriptable data = null;
                if(keyID >= 0)
                    data = this._upgradeStructure[keyID];

                this._castle.AddNewUpgrades(data);

                foreach(TestResearchUpgradeCard card in this._upgradeCardsToDisplay)
                    card.HideCard();

                this._backButton.gameObject.SetActive(false);

                this._state = ResearchState.FINISHED;
            }
        }

        public bool IsResearchPhase(int currentTurn) {

            if(this.ResearchCount > this._researchTurns.Length) {
                this._state = ResearchState.CLASS;
                this._researchState = ResearchState.UPGRADE;
                this._isUnitResearch = false;
                return false;
            }

            if(currentTurn == this._researchTurns[this._researchCount]) {
                this._researchCount++;
                this._state = ResearchState.CLASS;
                this._researchState = ResearchState.UNITS;
                this._isUnitResearch = true;
                return true;
            }

            this._castle.DebugUpgradeList();

            this._state = ResearchState.CLASS;
            this._researchState = ResearchState.UPGRADE;
            this._isUnitResearch = false;
            return false;
        }

        public void RepositionCards() {
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

        public void RepositionUpgradeCards() {
            float startPos = -200.0f; // For 3 cards.

            for(int i = 0; i < this._upgradeCardsToDisplay.Count; i++) {
                if(this._upgradeCardsToDisplay.Count == 1) {
                    this._upgradeCardsToDisplay[i].SetPosition(Vector3.zero);
                } else if(this._upgradeCardsToDisplay.Count == 2) {
                    if(i == 0) // Left
                        this._upgradeCardsToDisplay[i].SetPosition(new Vector3(-100.0f, 0.0f, 0.0f));
                    else // Right
                        this._upgradeCardsToDisplay[i].SetPosition(new Vector3(100.0f, 0.0f, 0.0f));
                } else if(this._upgradeCardsToDisplay.Count > 2) {
                    this._upgradeCardsToDisplay[i].SetPosition(new Vector3(startPos, 0.0f, 0.0f));
                    startPos += 200.0f;
                }
            }
        }

        private void DebugUnitList() {
            /*foreach(ClassType type in this._unitCardsOld.Keys) {
                List<TestResearchCard> cards = this._unitCardsOld[type];

                foreach(TestResearchCard card in cards) {
                    Debug.Log(type.ToString() + " - " + card.UnitType.ToString());
                }
            }*/

            /*foreach(ClassType classType in this._unitCards.Keys) {
                this._unitCards[classType].DebugList();
            }*/

            foreach(ClassType classType in this._upgradeCards.Keys) {
                Debug.Log("class Type: " + classType.ToString() + " - Count: " + this._upgradeCards.Count.ToString());
                this._upgradeCards[classType].DebugList();
            }
        }
    }
}