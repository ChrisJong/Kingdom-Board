namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Player;
    using Enum;

    public class CastleUI : MonoBehaviour {
        public Player controller;

        public GameObject goGroup;

        public Button spawnButtonArcher;
        public Button spawnButtonMage;
        public Button spawnButtonWarrior;

        #region UNITY
        private void Awake() {
            this.spawnButtonArcher.onClick.AddListener(delegate { this.AddToQueue(UnitType.ARCHER); });
            this.spawnButtonMage.onClick.AddListener(delegate { this.AddToQueue(UnitType.MAGE); });
            this.spawnButtonWarrior.onClick.AddListener(delegate { this.AddToQueue(UnitType.WARRIOR); });
            
            this.goGroup.SetActive(false);
        }
        #endregion

        #region CLASS
        private void AddToQueue(UnitType type) {
            Debug.Log("ADDING " + type.ToString() + " TO " + this.controller.name + " QUEUE");

            // NOTE: Add to castle queue let the castle handle the spawning of the objects.
        }
        #endregion
    }
}