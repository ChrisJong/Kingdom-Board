namespace UI {

    using UnityEngine;
    using UnityEngine.UI;

    using Constants;
    using Enum;
    using Structure;

    public class CastleUI : ScreenSpaceUI {

        #region VARIABLE
        public Castle castle;

        public Text textInfo;

        public Button btnSpawn;
        public Button btnBack;

        public Button _btnSpawnArcher;
        public Button _btnSpawnCrossbow;
        public Button _btnSpawnLongbow;

        public Button _btnSpawnMage;
        public Button _btnSpawnCleric;
        public Button _btnSpawnWizard;

        public Button _btnSpawnWarrior;
        public Button _btnSpawnKnight;
        public Button _btnSpawnGuardian;

        private GameObject _spawnParent;

        //private List<Button> _listSpawns;

        private bool _showSpawn = false;
        private bool _showDefault = true;
        #endregion

        #region UNITY
        protected override void Awake() {
            if(this.castle == null) {
                this.castle = this.transform.GetComponent<Castle>();
                this.controller = this.castle.controller;
            }

            this.FindUI(this.transform, UIValues.Structure.CASTLEUI);
            base.Awake();

            this.Init();
            
            this.ResetUI();
        }

        #endregion

        #region CLASS
        private void Init() {
            if(this.btnSpawn == null)
                this.btnSpawn = this._tSelected.Find(UIValues.Structure.SPAWNBUTTON).GetComponent<Button>();

            if(this._tSelected.Find(UIValues.Structure.SPAWNGROUP) != null)
                this._spawnParent = this._tSelected.Find(UIValues.Structure.SPAWNGROUP).gameObject;

            if(this.btnBack == null)
                this.btnBack = this.FindButton(this._spawnParent.transform, UIValues.Structure.BACKBUTTON);

            if(this._btnSpawnMage == null)
                this._btnSpawnMage = this.FindButton(this._spawnParent.transform, UIValues.Structure.SPAWNMAGE);

            if(this._btnSpawnCleric == null)
                this._btnSpawnCleric = this.FindButton(this._spawnParent.transform, UIValues.Structure.SPAWNCLERIC);

            if(this._btnSpawnWizard == null)
                this._btnSpawnWizard = this.FindButton(this._spawnParent.transform, UIValues.Structure.SPAWNWIZARD);

            if(this._btnSpawnArcher == null)
                this._btnSpawnArcher = this.FindButton(this._spawnParent.transform, UIValues.Structure.SPAWNARCHER);

            if(this._btnSpawnCrossbow == null)
                this._btnSpawnCrossbow = this.FindButton(this._spawnParent.transform, UIValues.Structure.SPAWNCROSSBOW);

            if(this._btnSpawnLongbow == null)
                this._btnSpawnLongbow = this.FindButton(this._spawnParent.transform, UIValues.Structure.SPAWNLONGBOW);

            if(this._btnSpawnWarrior == null)
                this._btnSpawnWarrior = this.FindButton(this._spawnParent.transform, UIValues.Structure.SPAWNWARRIOR);

            if(this._btnSpawnKnight == null)
                this._btnSpawnKnight = this.FindButton(this._spawnParent.transform, UIValues.Structure.SPAWNKNIGHT);

            if(this._btnSpawnGuardian == null)
                this._btnSpawnGuardian = this.FindButton(this._spawnParent.transform, UIValues.Structure.SPAWNGUARdIAN);

            this.btnSpawn.onClick.AddListener(this.ShowSpawn);
            this.btnBack.onClick.AddListener(this.GoBack);

            this._btnSpawnArcher.onClick.AddListener(delegate { this.AddToQueue(UnitType.ARCHER); });
            this._btnSpawnCrossbow.onClick.AddListener(delegate { this.AddToQueue(UnitType.CROSSBOW); });
            this._btnSpawnLongbow.onClick.AddListener(delegate { this.AddToQueue(UnitType.LONGBOW); });

            this._btnSpawnMage.onClick.AddListener(delegate { this.AddToQueue(UnitType.MAGE); });
            this._btnSpawnCleric.onClick.AddListener(delegate { this.AddToQueue(UnitType.CLERIC); });
            this._btnSpawnWizard.onClick.AddListener(delegate { this.AddToQueue(UnitType.WIZARD); });

            this._btnSpawnWarrior.onClick.AddListener(delegate { this.AddToQueue(UnitType.WARRIOR); });
            this._btnSpawnKnight.onClick.AddListener(delegate { this.AddToQueue(UnitType.KNIGHT); });
            this._btnSpawnGuardian.onClick.AddListener(delegate { this.AddToQueue(UnitType.GUARDIAN); });

            if(this.textInfo == null)
                this.textInfo = this._tHover.Find("Info_TEXT").GetComponent<Text>();
        }

        public override void UpdateUI() {
            base.UpdateUI();

            this.UpdateInfo();
        }

        public override void Display() {
            base.Display();

            if(this._goSelected.activeSelf)
                return;

            this._goSelected.SetActive(true);
        }

        public override void Hide() {
            base.Hide();

            if(!this._goSelected.activeSelf)
                return;

            this.ResetUI();
            this._goSelected.SetActive(false);

            if(this._goHover.activeSelf)
                this._goHover.SetActive(false);
        }

        protected override void ResetUI() {
            this._spawnParent.SetActive(false);

            this.btnSpawn.gameObject.SetActive(true);
            this._showSpawn = false;
            this._showDefault = true;
        }

        private void ShowSpawn() {
            if(this._showDefault) {
                this._spawnParent.SetActive(true);

                this.btnSpawn.gameObject.SetActive(false);
                this._showSpawn = true;
                this._showDefault = false;
            }
        }

        private void GoBack() {
            if(this._showSpawn) {
                this._spawnParent.SetActive(false);

                this.btnSpawn.gameObject.SetActive(true);
                this._showSpawn = false;
                this._showDefault = true;
            }
        }

        private void AddToQueue(UnitType type) {
            Debug.Log("TRYING TO ADD " + type.ToString() + " TO " + this.controller.name + " QUEUE");
            if(!this.castle.AddUnitToQueue(type))
                Debug.Log("UNABLE TO ADD " + type.ToString() + " TO QUEUE");

            this.UpdateInfo();
        }

        private void UpdateInfo() {
            string text = string.Empty;
            string spawnlist = string.Empty;

            text = "HEALTH: " + this.castle.currentHealth + " / " + this.castle.maxHealth + "\r\n" +
                   "SPAWN LIST: " + "\r\n";

            if(this.castle.spawnQueue.Count != 0) {
                foreach(SpawnQueueType spawn in castle.spawnQueue) {
                    spawnlist += spawn.type.ToString() + " - " + spawn.counter + " Round Until Spawn" + "\r\n";
                }
                text += spawnlist;
            }

            this.textInfo.text = text;
        }
        #endregion
    }
}