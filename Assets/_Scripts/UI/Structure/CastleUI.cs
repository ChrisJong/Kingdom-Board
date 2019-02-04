namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using TMPro;

    using Enum;
    using Manager;
    using Scriptable;
    using Structure;
    using Player;
    using System;

    [System.Serializable]
    public class CastleUI : UIBase {

        #region INFO_UI

        private GameObject _infomationGroup;
        [SerializeField] private GameObject _healthGroup;
        private TextMeshProUGUI _healthText;
        private RectTransform _healthBarTransform;

        [SerializeField] private GameObject _hoverUI = null;
        [SerializeField] private RectTransform _hoverUITransform = null;
        [SerializeField] private Canvas _hoverUICanvas = null;
        [SerializeField] private RectTransform _hoverHealthBarTransform = null;

        [SerializeField] private bool _hoverTrigger = false;

        public bool HoverTrigger { get { return this._hoverTrigger; } }

        #endregion

        #region VARIABLE
        [SerializeField] private Castle _castle;

        [SerializeField] private List<QueueButton> _queueList = new List<QueueButton>();
        [SerializeField] private List<TrainButton> _trainListButtons = new List<TrainButton>();

        IEnumerator _panelAnimation;

        [SerializeField] private bool _spawnGroupToggle = false;
        [SerializeField] private bool _spawnGroupMoving = false;

        [Header("Unit Training Panel")]
        [SerializeField] private float _showPanelPosX = 420.0f;
        [SerializeField] private float _hidePanelPoxX = -270.0f;
        [SerializeField] private float _panelMoveSpeed = 50.0f;
        private float _trainingButtonSize = 50.0f;
        [SerializeField] private float _trainingTierPadding = 120.0f;
        [SerializeField] private float _trainingPadiding = 10.0f;
        private float _queueStartPosY = 0.0f;
        private float _queueHeight = 60.0f;
        [SerializeField] private float _queueVerticalPadding = 2.0f;
        [SerializeField] private float _queueMoveSpeed = 10.0f;

        [Space]
        [SerializeField] private GameObject _trainUnitPrefab;
        [SerializeField] private GameObject _queueRibbonPrefab;

        private RectTransform _spawnGroup = null;
        private RectTransform _spawnQueueGroup = null;
        private RectTransform _spawnListGroup = null;
        private RectTransform _spawnBackground = null;

        private RectTransform _meleePanel = null;
        private RectTransform _meleeButtonPanel = null;
        private RectTransform _rangePanel = null;
        private RectTransform _rangeButtonPanel = null;
        private RectTransform _magicPanel = null;
        private RectTransform _magicButtonPanel = null;

        public bool SpawnGroupMoving { get { return this._spawnGroupMoving; } }
        public bool SpawnGroupToggle { get { return this._spawnGroupToggle; } }

        #endregion

        #region UNITY

        #endregion

        #region CLASS
        public void Setup(Castle castle) {
            this._castle = castle;

            this.Setup();
        }

        public override void Init(Player controller) {
            base.Init(controller);

            // Information Group
            this._infomationGroup = this._controller.playerUI.structureUIGroup;
            this._healthGroup = this._infomationGroup.transform.Find("BG").Find("Health").gameObject;
            this._healthBarTransform = this._healthGroup.transform.Find("Bar").transform as RectTransform;

            //Debug.Log("Health Bar Size Delta: " + this._healthBarTransform.sizeDelta.ToString());
            //Debug.Log("Health Bar Rect: " + this._healthBarTransform.rect.ToString());

            this._healthText = this._healthGroup.transform.Find("Text").GetComponent<TextMeshProUGUI>() as TextMeshProUGUI;
            if(this._hoverUI == null) {
                this._hoverUI = this.transform.Find("_UIHover").gameObject;
                this._hoverUICanvas = this._hoverUI.GetComponent<Canvas>() as Canvas;
                this._hoverUITransform = this._hoverUI.transform as RectTransform;
            }
            this._hoverUICanvas.worldCamera = controller.playerCamera.mainCamera;
            this._hoverUI.SetActive(false);

            this._panelAnimation = this.MoveTrainingPanel();
            this._spawnGroup = this._controller.playerUI.spawnGroup.transform as RectTransform;

            this._spawnQueueGroup = this._spawnGroup.Find("Queue_Panel") as RectTransform;
            this._spawnListGroup = this._spawnGroup.Find("UnitTrain_Panel") as RectTransform;
            this._spawnBackground = this._spawnGroup.Find("Background") as RectTransform;
            this._spawnListGroup.gameObject.AddComponent<BookToggle>().Init(this);
            this._spawnBackground.gameObject.AddComponent<TrainBookToggle>().Init(this);

            this._meleePanel = this._spawnListGroup.Find("Melee_Panel") as RectTransform;
            this._meleeButtonPanel = this._meleePanel.Find("UnitButton_Panel") as RectTransform;
            this._rangePanel = this._spawnListGroup.Find("Range_Panel") as RectTransform;
            this._rangeButtonPanel = this._rangePanel.Find("UnitButton_Panel") as RectTransform;
            this._magicPanel = this._spawnListGroup.Find("Magic_Panel") as RectTransform;
            this._magicButtonPanel = this._magicPanel.Find("UnitButton_Panel") as RectTransform;

            this.GenerateSpawnListButtons();

            this.Hide();
        }

        public override void Return() {
            this._hoverUICanvas.worldCamera = null;

            base.Return();
        }

        public override void Display() {
            if(!this._spawnGroupToggle)
                this.ToggleSpawnGroup(true);

            this.UpdateUI();
            this._infomationGroup.SetActive(true);
        }

        public override void Hide() {

            if(this._spawnGroupToggle)
                this.ToggleSpawnGroup(false);

            this._infomationGroup.SetActive(false);

            this._castle.radiusDrawer.SetActive(false);
        }

        public override void OnEnter() {
        }

        public override void OnEnter(Player controller) {

            this.UpdateHoverUI();
            this._hoverUI.SetActive(true);
        }

        public override void OnExit() {

            this._hoverUI.SetActive(false);
            //this._infomationGroup.SetActive(false);
        }

        public override void ResetUI() {
            this._infomationGroup.SetActive(false);
            this.ResetSpawnGroup();
        }
            
        public override void UpdateUI() {
            // information Group
            float healthBarWidth = 290.0f; // Stored Width of the health Bar.
            float newHealth = healthBarWidth * (this._castle.CurrentHealth / this._castle.MaxHealth);
            Vector2 newRect = new Vector2(newHealth, this._healthBarTransform.sizeDelta.y);
            this._healthBarTransform.sizeDelta = newRect;
            this._healthText.text = this._castle.CurrentHealth.ToString() + " / " + this._castle.MaxHealth.ToString();
        }

        public void UpdateHoverUI() {
            float healthBarWidth = 4.0f;
            float newHeatlh = healthBarWidth * (this._castle.CurrentHealth / this._castle.MaxHealth);
            Vector2 newRect = new Vector2(newHeatlh, this._hoverHealthBarTransform.sizeDelta.y);
            this._hoverHealthBarTransform.sizeDelta = newRect;
        }

        public void AddToQueue(UnitScriptable unitData, SpawnQueueType queueType) {

            float yPos = this._queueStartPosY - this._queueList.Count * (this._queueHeight + this._queueVerticalPadding);

            GameObject go = Instantiate(this._queueRibbonPrefab, this._spawnQueueGroup);
            QueueButton queueButton = go.GetComponent<QueueButton>() as QueueButton;

            queueButton.Init(yPos, this._castle, queueType, unitData.ClassData.classColor, unitData.iconSprite);
            queueType.SetQueueButton(queueButton);
            this._queueList.Add(queueButton);
        }

        public void RemoveFromQueue(SpawnQueueType queue) {

            if(this._queueList.Contains(queue.queueButton)) {

                this._queueList.Remove(queue.queueButton);
                this.SortQueue();

            } else
                Debug.LogError("There is no Queue For Spawning, " + "ID: " + queue.id.ToString() + " - " + queue.type);
        }

        public void SortQueue() {
            for(int i = 0; i < this._queueList.Count; i++) {
                float yPos = this._queueStartPosY - i * (this._queueHeight + this._queueVerticalPadding);
                StartCoroutine(this._queueList[i].MoveButton(yPos, this._queueMoveSpeed));
            }
        }

        public void UnlockSpawnButton(UnitClassType classType, UnitType unitType) {
            // Search for the button using the class and unit type.
            foreach(TrainButton button in this._trainListButtons) {
                if(button.UnitType == unitType) {
                    button.Unlock();
                }
            }
        }

        public void ToggleSpawnGroup(bool active) {
            if(this._spawnGroupToggle == active)
                return;

            this._spawnGroupToggle = active;

            if(this._spawnGroupMoving)
                StopCoroutine(this._panelAnimation);

            this._panelAnimation = null;
            this._panelAnimation = this.MoveTrainingPanel();
            StartCoroutine(this._panelAnimation);
        }

        public void ToggleSpawnGroup() {

            this._spawnGroupToggle = !this._spawnGroupToggle;

            if(this._spawnGroupMoving)
                StopCoroutine(this._panelAnimation);

            this._panelAnimation = null;
            this._panelAnimation = this.MoveTrainingPanel();
            StartCoroutine(this._panelAnimation);
        }

        private void ResetSpawnGroup() {

            if(this._spawnGroupMoving)
                StopCoroutine(this._panelAnimation);

            Vector3 tempPos = this._spawnGroup.anchoredPosition;

            this._spawnGroupMoving = false;
            this._spawnGroupToggle = false;
            this._panelAnimation = null;

            tempPos.x = this._hidePanelPoxX;
            this._spawnGroup.anchoredPosition = tempPos;
        }

        private void GenerateSpawnListButtons() {

            int max = System.Enum.GetNames(typeof(UnitClassType)).Length - 2;

            for(int i = 0; i < max; i++) {

                UnitClassType classType = ((UnitClassType)i + 1);
                List<UnitScriptable> unitList = UnitPoolManager.instance.SortedUnitClass[classType];

                for(int j = 0; j < unitList.Count; j++) {

                    UnitScriptable unit = unitList[j];
                    GameObject go = Instantiate(this._trainUnitPrefab);
                    TrainButton btn = go.GetComponent<TrainButton>() as TrainButton;
                    RectTransform rect = go.transform as RectTransform;

                    go.name = unit.unitType.ToString() + "_BTN";
                    btn.Init(this._castle, unit, Constants.GlobalSettings.Debugging.lockedTraining);

                    this._trainListButtons.Add(btn);

                    if(classType == UnitClassType.MELEE)
                        rect.SetParent(this._meleeButtonPanel);
                    else if(classType == UnitClassType.RANGE)
                        rect.SetParent(this._rangeButtonPanel);
                    else if(classType == UnitClassType.MAGIC)
                        rect.SetParent(this._magicButtonPanel);
                    else
                        Debug.LogError("Not Class Type Exists for this unit: " + classType.ToString());

                    rect.anchoredPosition = Vector2.zero;

                    if(j == 1) {

                        rect.anchoredPosition = new Vector2(this._trainingTierPadding, 0.0f);

                    } else if(j > 1) {

                        float xPos = (this._trainingButtonSize * (j - 1)) + this._trainingPadiding;
                        rect.anchoredPosition = new Vector2(this._trainingTierPadding + xPos, 0.0f);

                    }

                }
            }
        }

        private IEnumerator MoveTrainingPanel() {
            this._spawnGroupMoving = true;

            if(this._spawnGroupToggle) { // Open

                while(this._spawnGroup.anchoredPosition.x < this._showPanelPosX) {

                    Vector3 panelPos = this._spawnGroup.anchoredPosition;
                    panelPos.x += this._panelMoveSpeed;
                    this._spawnGroup.anchoredPosition = panelPos;

                    if(panelPos.x >= this._showPanelPosX) {

                        panelPos.x = this._showPanelPosX;
                        this._spawnGroup.anchoredPosition = panelPos;
                        break;
                    }

                    yield return new WaitForEndOfFrame();
                }

            } else { // Close

                while(this._spawnGroup.anchoredPosition.x > this._hidePanelPoxX) {

                    Vector3 panelPos = this._spawnGroup.anchoredPosition;
                    panelPos.x -= this._panelMoveSpeed;
                    this._spawnGroup.anchoredPosition = panelPos;

                    if(panelPos.x <= this._hidePanelPoxX) {

                        panelPos.x = this._hidePanelPoxX;
                        this._spawnGroup.anchoredPosition = panelPos;
                        break;
                    }

                    yield return new WaitForEndOfFrame();
                }
            }

            this._spawnGroupMoving = false;
            yield return null;
        }

        #endregion
    }
}