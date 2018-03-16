﻿namespace Player {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Manager;
    using Selectable;
    using Selectable.Structure;
    using Selectable.Unit;

    public abstract class Player : MonoBehaviour {

        #region VARIABLE
        public int id { private set; get; }
        public string name;

        public bool IsAttacking { get; private set; }
        public bool TurnEnded { get; private set; }

        public GameObject PlayerObject { get; private set; }
        private GameObject _castle;
        private Transform _spawnLocation;

        // CAMERA
        private Camera _playerCamnera;
        private GameObject _cameraObject;

        // UI
        private Canvas ui;
        #endregion

        #region METHOD
        public virtual void Init(GameObject playerGO, Transform spawnLocation, int id = 0, bool onAttack = false) {
            this.id = id;
            this.name = "Player " + (id + 1).ToString().PadLeft(2, '0');

            this.PlayerObject = playerGO;
            this.PlayerObject.transform.SetPositionAndRotation(spawnLocation.position, spawnLocation.rotation);

            this._spawnLocation = spawnLocation;

            this._castle = GameObject.Instantiate(AssetManager.instance.castle, this.PlayerObject.transform.position, this.PlayerObject.transform.rotation, this.PlayerObject.transform);
            this._castle.GetComponent<Castle>().Init(this);

            PlayerCamera.CreateCamera(this, spawnLocation, onAttack);
        }

        public abstract void UpdatePlayer();
        
        public void EndTurn() {
            if(this.TurnEnded)
                return;
            else
                this.TurnEnded = true;
        }

        #endregion
    }
}