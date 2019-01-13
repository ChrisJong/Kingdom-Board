namespace Player {

    using UnityEngine;

    using Constants;
    using Enum;

    [System.Serializable]
    public class PlayerResources {
        [SerializeField] private int _gold = 0;
        [SerializeField] private int _population = 0;
        [SerializeField] private int _populationCap = 0;

        public void Init() {
            this._gold = PlayerValues.STARTGOLD;
            this._population = 0;
            this._populationCap = PlayerValues.POPULATIONCAP;
        }

        public bool SpendResource(PlayerResource resource, int value) {
            if(resource == PlayerResource.GOLD) {
                if(this._gold < value)
                    return false;
                else {
                    this.RemoveResource(resource, value);
                }
            } else if(resource == PlayerResource.POPULATION) {
                if((this._population + value) > this._populationCap)
                    return false;
                else {
                    this.AddResource(resource, value);
                }
            }

            return true;
        }

        public void ChangePopulationCap(int value) {
            this._populationCap = value;
        }

        public int GetResource(PlayerResource resource) {
            int value = 0;

            if(resource == PlayerResource.GOLD)
                value = this._gold;
            else if(resource == PlayerResource.POPULATION)
                value = this._population;
            else
                value = -1;

            return value;
        }

        public void AddResource(PlayerResource resource, int value) {
            if(resource == PlayerResource.GOLD)
                this._gold += value;
            else if(resource == PlayerResource.POPULATION) {

                if((this._population + value) > this._populationCap) {
                    this._population = _populationCap;
                } else
                    this._population += value;
            }
        }

        public void RemoveResource(PlayerResource resource, int value) {
            if(resource == PlayerResource.GOLD) {
                this._gold -= value;

                if(this._gold < 0)
                    this._gold = 0;

            } else if(resource == PlayerResource.POPULATION) {
                this._population -= value;

                if(this._population < 0)
                    this._population = 0;
            }
        }
    }
}