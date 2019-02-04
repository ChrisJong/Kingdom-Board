namespace Manager {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Extension;
    using Helpers;
    using Structure;
    using Utility;

    public sealed class GoldMineManager : SingletonMono<GoldMineManager> {

        [SerializeField] private int _mineCount = 0;

        [SerializeField] private List<GoldMine> _goldMineList = new List<GoldMine>();

        [SerializeField] private GameObject _goldMinePrefab;

        [SerializeField] private Transform _spawnPointGroup;
        [SerializeField] private Transform[] _spawnPoints;

        public override void Init() {

            this._spawnPoints = this._spawnPointGroup.GetComponentsInChildren<Transform>();

            // Spawn Gold Mines in the game.
            foreach(Transform spawnPoint in this._spawnPoints) {
                if(spawnPoint == this._spawnPointGroup)
                    continue;

                Vector3 point = spawnPoint.position;

                GameObject temp = GameObject.Instantiate(this._goldMinePrefab, point, Quaternion.identity, this.transform);

                GoldMine script = temp.GetComponent<GoldMine>() as GoldMine;

                if(script != null)
                    this._goldMineList.Add(script);
                else {
                    script = temp.AddComponent<GoldMine>();
                    this._goldMineList.Add(script);
                }

                script.Setup();
                script.Init(GameManager.instance.Players);

                this._mineCount++;
            }
        }

        public int CheckRound(Player.Player controller) {

            int gold = 0;

            foreach(GoldMine mine in this._goldMineList) {
                if(mine.PlayerInControl == controller) {
                    gold += mine.Gold;
                }
            }

            return gold;
        }

        public void RemoveEntity(HasHealthBase entity) {
            foreach(GoldMine mine in this._goldMineList) {
                mine.RemoveEntity(entity);
            }
        }
    }
}