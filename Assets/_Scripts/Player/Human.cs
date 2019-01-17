namespace Player {

    using UnityEngine;

    using UI;

    public class Human : Player {

        #region CLASS

        public override void Create(Transform spawnLocation, uint id = 0) {

            if(this.transform.GetComponent<PlayerUI>() as PlayerUI != null)
                this._ui = this.transform.GetComponent<PlayerUI>() as PlayerUI;
            else
                this._ui = this.gameObject.AddComponent<PlayerUI>();

            this._ui.Setup();
            this._ui.Init(this);

            base.Create(spawnLocation, id);
        }

        public override void UpdatePlayer() {

        }

        #endregion
    }
}