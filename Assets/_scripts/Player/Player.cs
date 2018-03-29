namespace Player {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Manager;
    using Selectable;
    using Selectable.Structure;
    using Selectable.Unit;
    using UI;

    public abstract class Player : MonoBehaviour {

        #region VARIABLE
        public int id { private set; get; }
        public uint roll = 0;
        public string name;

        public bool IsAttacking { get; private set; }
        public bool TurnEnded { get; private set; }

        public GameObject PlayerGO { get; private set; }
        private GameObject _castle;
        public GameObject Castle { get { return this._castle; } }
        private Transform _spawnLocation;

        // CAMERA
        private Camera _cameraScript;
        public GameObject _cameraGO;

        // UI
        private PlayerUI _playerUi;
        #endregion

        #region CLASS
        public virtual void Create(GameObject playerGO, Transform spawnLocation, int id = 0) {
            this.id = id;
            this.name = "Player " + (id + 1).ToString().PadLeft(2, '0');
            this.TurnEnded = false;

            this.PlayerGO = playerGO;
            this.PlayerGO.transform.SetPositionAndRotation(spawnLocation.position, spawnLocation.rotation);

            this._spawnLocation = spawnLocation;

            this._castle = GameObject.Instantiate(AssetManager.instance.castle, this.PlayerGO.transform.position, this.PlayerGO.transform.rotation, this.PlayerGO.transform);
            this._castle.GetComponent<Castle>().Init(this);

            GameObject tempUI = GameObject.Instantiate(AssetManager.instance.playerUI, this.transform);
            this._playerUi = tempUI.AddComponent<PlayerUI>();
            this._playerUi.Init(this);

            
        }
        public virtual void Init(bool attacking) {
            this.IsAttacking = attacking;

            if(attacking)
                this._playerUi.DisplayUI();
            else
                this._playerUi.HideUI();

            this._cameraGO = PlayerCamera.CreateCamera(this, this._spawnLocation, attacking);
        }

        public abstract void UpdatePlayer();
        
        public virtual void NewTurn(bool attacking) {
            this.IsAttacking = attacking;
            this.TurnEnded = false;
            this._cameraGO.SetActive(attacking);
            this._playerUi.gameObject.SetActive(attacking);
        }

        public virtual void EndTurn() {
            if(this.TurnEnded)
                return;

            this.TurnEnded = true;
            GameManager.instance.CheckRound();
        }

        #endregion
    }
}