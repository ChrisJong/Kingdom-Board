namespace Player {

    using UnityEngine;

    using UI;

    [RequireComponent(typeof(PlayerCursor))]
    public class Human : Player {

        #region VARIABLE

        private PlayerCursor _playerCursor;

        public PlayerCursor playerCursor { get { return this._playerCursor; } }

        #endregion

        #region CLASS

        public override void EndTurn() {
            this._playerCursor.SetDefault();

            base.EndTurn();
        }

        public override void Create(Transform spawnLocation, uint id = 0) {

            if(this._playerCursor == null)
                if(this.GetComponent<PlayerCursor>() != null)
                    this._playerCursor = this.GetComponent<PlayerCursor>() as PlayerCursor;

            this._playerCursor.SetDefault();

            if(this.transform.GetComponent<PlayerUI>() as PlayerUI != null)
                this._playerUI = this.transform.GetComponent<PlayerUI>() as PlayerUI;
            else
                this._playerUI = this.gameObject.AddComponent<PlayerUI>();

            this._playerUI.Setup();

            base.Create(spawnLocation, id);

            this._playerUI.Init(this);
        }

        public override void UpdatePlayer() {

        }

        #endregion
    }
}