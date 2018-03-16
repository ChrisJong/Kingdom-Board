namespace Selectable.Structure {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Unit;

    public class Castle : Structure {
        private float _spawnDistannce = 6.0f;
        private float _anglePerSpawn = 35f;

        public List<Unit> _spawnQueue = new List<Unit>();

        #region UNITY
        private void Awake() {
            this.UI.gameObject.SetActive(false);
        }

        protected override void OnMouseEnter() {

        }

        protected override void OnMouseExit() {
            
        }
        #endregion

        #region CLASS
        public override void Init() {
            this._name = (this._player.name) + " Castle";
        }

        public void UpdateSpawnQueue() {

        }

        public void AddToSpawnQueue(Unit unit) {
            this._spawnQueue.Add(unit);
        }

        public void HandleSpawnUnit(Unit unit) {

        }

        public override void DisplayUI() {
            this.UI.gameObject.SetActive(true);
        }

        public override void HideUI() {
            this.UI.gameObject.SetActive(false);
        }
        #endregion
    }
}