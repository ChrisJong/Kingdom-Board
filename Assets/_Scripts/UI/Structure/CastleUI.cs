namespace UI {

    using System.Collections;
    using System.Collections.Generic;

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

        public Button btnSpawnProjectile;
        public Button btnSpawnMagic;
        public Button btnSpawnPhysical;
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

            if(this.btnSpawnMagic == null)
                this.btnSpawnMagic = this.FindButton(this._spawnParent.transform, "Magic_BTN");

            if(this.btnSpawnProjectile == null)
                this.btnSpawnProjectile = this.FindButton(this._spawnParent.transform, "Projectile_BTN");

            if(this.btnSpawnPhysical == null)
                this.btnSpawnPhysical = this.FindButton(this._spawnParent.transform, "Physical_BTN");

            this.btnSpawn.onClick.AddListener(this.ShowSpawn);
            this.btnBack.onClick.AddListener(this.GoBack);

            this.btnSpawnProjectile.onClick.AddListener(delegate { this.AddToQueue(UnitType.ARCHER); });
            this.btnSpawnMagic.onClick.AddListener(delegate { this.AddToQueue(UnitType.MAGE); });
            this.btnSpawnPhysical.onClick.AddListener(delegate { this.AddToQueue(UnitType.WARRIOR); });
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
            Debug.Log("ADDING " + type.ToString() + " TO " + this.controller.name + " QUEUE");
            if(!this.castle.AddUnitToQueue(type))
                Debug.Log("UNABLE TO ADD " + type.ToString() + " TO QUEUE");
            // NOTE: Add to castle queue let the castle handle the spawning of the objects.
        }
        #endregion
    }
}