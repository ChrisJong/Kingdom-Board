namespace Player {

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    using Castle;

    public class Player : MonoBehaviour {

        public PlayerInfo _playerData;

        public List<GameObject> _units;

        public Vector3 _castleSpawnPoint;
        public GameObject _castle;

        public GameObject _camera;

        #region UNITY_METHODS
        void Awake() {
            
        }
        #endregion

        #region CLASS_METHODS
        public void init(GameObject castleSpawn, int id) {
            this._castleSpawnPoint = castleSpawn.transform.position;

            this._playerData.id = id;

            this._camera = PlayerCamera.CreateCamera(this._playerData.id);
            this._camera.transform.parent = this.transform;

            this.SpawnCastle();
        }

        private void SpawnCastle() {
            Instantiate(AssetPool.instance.castle, new Vector3(this._castleSpawnPoint.x, AssetPool.instance.castle.transform.position.y, this._castleSpawnPoint.z), Quaternion.identity, this.transform);
        }
        #endregion
    }
}