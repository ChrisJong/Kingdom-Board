namespace UI {

    using UnityEngine;
    using UnityEngine.UI;

    using Enum;
    using Player;
    using Structure;
    using System;

    public class CastleUI : UIComponent {

        #region VARIABLE
        public Castle castle;

        public Button btnSpawn;
        public Button btnBack;

        public Button btnSpawnProjectile;
        public Button btnSpawnMagic;
        public Button btnSpawnPhysical;
        private GameObject _spawnParent;

        private bool _showSpawn = false;
        private bool _showDefault = true;
        #endregion

        #region UNITY
        private void Awake() {
            this.btnSpawn.onClick.AddListener(this.ShowSpawn);
            this.btnBack.onClick.AddListener(this.GoBack);

            this._spawnParent = this.btnSpawnMagic.transform.parent.gameObject;
            this.btnSpawnProjectile.onClick.AddListener(delegate { this.AddToQueue(UnitType.ARCHER); });
            this.btnSpawnMagic.onClick.AddListener(delegate { this.AddToQueue(UnitType.MAGE); });
            this.btnSpawnPhysical.onClick.AddListener(delegate { this.AddToQueue(UnitType.WARRIOR); });

            this.ResetUI();
            //this.isActive = false;
        }
        #endregion

        #region CLASS
        public override void DisplayUI() {
            this.isActive = true;
        }

        public override void HideUI() {
            this.ResetUI();
            this.isActive = false;
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