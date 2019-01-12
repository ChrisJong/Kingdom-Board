namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Enum;
    using Manager;
    using Player;
    using Unit;
    using Scriptable;
    using Structure;

    public class CastleUI : ScreenSpace {

        #region VARIABLE
        private Castle _castle;

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

        #endregion

        #region UNITY

        #endregion

        #region CLASS
        public void Init(Castle castle) {

            this.Init(castle.controller);

            this._castle = castle;

            this._panelAnimation = this.MoveTrainingPanel();
            this._spawnGroup = this._controller.UI.spawnGroup.transform as RectTransform;

            this._spawnQueueGroup = this._spawnGroup.Find("Queue_Panel") as RectTransform;
            this._spawnListGroup = this._spawnGroup.Find("UnitTrain_Panel") as RectTransform;
            this._spawnBackground = this._spawnGroup.Find("Background") as RectTransform;
            this._spawnListGroup.gameObject.AddComponent<TrainBookToggle>().Init(this);
            this._spawnBackground.gameObject.AddComponent<TrainBookToggle>().Init(this);

            this._meleePanel = this._spawnListGroup.Find("Melee_Panel") as RectTransform;
            this._meleeButtonPanel = this._meleePanel.Find("UnitButton_Panel") as RectTransform;
            this._rangePanel = this._spawnListGroup.Find("Range_Panel") as RectTransform;
            this._rangeButtonPanel = this._rangePanel.Find("UnitButton_Panel") as RectTransform;
            this._magicPanel = this._spawnListGroup.Find("Magic_Panel") as RectTransform;
            this._magicButtonPanel = this._magicPanel.Find("UnitButton_Panel") as RectTransform;

            this.GenerateSpawnListButtons();

            this.HideUI();
        }

        public override void DisplayUI() {
            this._spawnGroup.gameObject.SetActive(true);
        }

        public override void HideUI() {
            Debug.Log("Hide UI");

            this.ResetSpawnGroup();
            this._spawnGroup.gameObject.SetActive(false);

            this._castle.radiusDrawer.SetActive(false);
        }

        public override void ResetUI() {
            this.ResetSpawnGroup();
        }
            
        public override void UpdateUI() {
            //throw new System.NotImplementedException();
        }

        public void AddToQueue(UnitScriptable unitData, SpawnQueueType queueType) {

            float yPos = this._queueStartPosY - this._queueList.Count * (this._queueHeight + this._queueVerticalPadding);

            GameObject go = Instantiate(this._queueRibbonPrefab, this._spawnQueueGroup);
            QueueButton queueButton = go.GetComponent<QueueButton>() as QueueButton;

            queueButton.Init(yPos, this._castle, queueType, unitData.ClassData.ClassColor, unitData.TrainIconSprite);
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

        public void UnlockSpawnButton(ClassType classType, UnitType unitType) {
            // Search for the button using the class and unit type.
            foreach(TrainButton button in this._trainListButtons) {
                if(button.UnitType == unitType) {
                    button.Unlock();
                }
            }
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

            int max = System.Enum.GetNames(typeof(ClassType)).Length - 2;

            for(int i = 0; i < max; i++) {

                ClassType classType = ((ClassType)i + 1);
                List<UnitScriptable> unitList = UnitPoolManager.instance.SortedUnitList[classType];

                for(int j = 0; j < unitList.Count; j++) {

                    UnitScriptable unit = unitList[j];
                    GameObject go = Instantiate(this._trainUnitPrefab);
                    TrainButton btn = go.GetComponent<TrainButton>() as TrainButton;
                    RectTransform rect = go.transform as RectTransform;

                    go.name = unit.unitType.ToString() + "_BTN";
                    btn.Init(this._castle, unit, true);
                    this._trainListButtons.Add(btn);

                    if(classType == ClassType.MELEE)
                        rect.SetParent(this._meleeButtonPanel);
                    else if(classType == ClassType.RANGE)
                        rect.SetParent(this._rangeButtonPanel);
                    else if(classType == ClassType.MAGIC)
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