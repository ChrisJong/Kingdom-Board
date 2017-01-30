namespace Player {

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    using Castle;

    public class Player : MonoBehaviour {

        public PlayerInfo _playerData = new PlayerInfo();

        public List<GameObject> _units;

        public GameObject _castleSpawn;
        public GameObject _castle;

        public GameObject _camera;

        #region UNITY_METHODS
        void Awake() {
            
        }
        #endregion

        #region PLAYER_METHODS
        public void init(GameObject castleSpawn, int id) {
            this._playerData.id = id;
            this._castleSpawn = castleSpawn;

            this._camera = PlayerCamera.CreateCamera(this._playerData.id);
            this._camera.transform.parent = this.transform;

            this.SpawnCastle();
        }

        public int Roll() {

            int temp = 0;

            temp = Random.Range(1, 10);

            return temp;
        }

        public void EndTurn() {
            if(this._playerData.turnEnded == false)
                this._playerData.turnEnded = true;
            else
                return;
        }

        private void SpawnCastle() {
        }
        #endregion

        #region DEBIG
        public void SwitchPlayers() {

        }
        #endregion
    }
}