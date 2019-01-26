namespace Manager {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Constants;
    using Enum;
    using Extension;
    using Player;

    public sealed class ResourceManager : SingletonMono<ResourceManager> {

        #region VARIABLE

        [SerializeField] private int _goldPerTurn = PlayerValues.GOLDPERTURN;
        [SerializeField] private int _populationCap = PlayerValues.POPULATIONCAP; 

        private Dictionary<Player, PlayerResources> _playerResources = new Dictionary<Player, PlayerResources>();

        public int GoldPerTurn { get { return this._goldPerTurn; } }
        public int PopulationCap { get { return this._populationCap; } }

        #endregion

        #region CLASS
        public override void Init() { }

        public void SetupPlayerResources(Player p) {
            PlayerResources res = new PlayerResources();
            res.Init();
            this._playerResources.Add(p, res);
        }

        public bool ChangePopulationCap(int value) {
            if(this._playerResources.Keys.Count == 0)
                return false;

            foreach(Player p in this._playerResources.Keys) {
                this._playerResources[p].ChangePopulationCap(value);
            }

            return true;
        }

        public void AddGoldPerTurn(Player p) {
            if(this._playerResources.Keys.Count == 0)
                return;
            else {

                if(this._playerResources.ContainsKey(p)) {
                    this._playerResources[p].AddResource(PlayerResource.GOLD, this._goldPerTurn);
                    this._playerResources[p].AddResource(PlayerResource.GOLD, GoldMineManager.instance.CheckRound(p));
                }
            }

        }

        public PlayerResources GetPlayerResources(Player p) {
            if(this._playerResources.ContainsKey(p)) {
                PlayerResources res = this._playerResources[p];
                return res;
            }

            return null;
        }

        public int GetPlayerResource(Player p, PlayerResource resource) {
            if(this._playerResources.ContainsKey(p)) {
               return this._playerResources[p].GetResource(resource);
            }

            return -1;
        }

        public bool AddResource(PlayerResource resource, int value) {
            if(this._playerResources.Keys.Count == 0)
                return false;

            foreach(Player p in this._playerResources.Keys) {
                PlayerResources res = this._playerResources[p];
                res.AddResource(resource, value);
            }

            return true;
        }

        public bool AddResource(Player p, PlayerResource resource, int value) {
            PlayerResources res;

            if(this._playerResources.ContainsKey(p)) {
                res = this._playerResources[p];
                res.AddResource(resource, value);
                return true;
            }

            return false;
        }

        public bool RemoveResource(PlayerResource resource, int value) {
            if(this._playerResources.Keys.Count == 0)
                return false;

            foreach(Player p in this._playerResources.Keys) {
                PlayerResources res = this._playerResources[p];
                res.RemoveResource(resource, value);
            }

            return true;
        }

        public bool RemoveResource(Player p, PlayerResource resource, int value) {
            PlayerResources res;

            if(this._playerResources.ContainsKey(p)) {
                res = this._playerResources[p];
                res.RemoveResource(resource, value);
                return true;
            }

            return false;
        }

        public bool SpendResource(Player p, PlayerResource resource, int value) {

            if(!this._playerResources.ContainsKey(p))
                return false;

            PlayerResources res = this._playerResources[p];
            return res.SpendResource(resource, value);

        } 
        #endregion
    }
}