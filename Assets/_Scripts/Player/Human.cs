namespace Player {

    using UnityEngine;

    using UI;

    [RequireComponent(typeof(PlayerUI))]
    [RequireComponent(typeof(PlayerSound))]
    [RequireComponent(typeof(PlayerCursor))]
    [RequireComponent(typeof(PlayerSelect))]
    [RequireComponent(typeof(PlayerCamera))]
    [RequireComponent(typeof(Research.Research))]
    public class Human : Player {

        #region VARIABLE
        private PlayerCursor _playerCursor;

        public PlayerCursor playerCursor { get { return this._playerCursor; } }
        #endregion

        #region UNITY
        private void Update() {
            this.UpdatePlayer();
        }
        #endregion

        #region CLASS

        public override void EndTurn() {
            this._playerCursor.ChangeState(Enum.CursorState.DEFAULT);

            base.EndTurn();
        }

        public override void Setup(Transform spawnLocation, uint id = 0) {

            if(this._playerCursor == null) {
                if(this.GetComponent<PlayerCursor>() == null)
                    this._playerCursor = this.gameObject.AddComponent<PlayerCursor>();
                else
                    this._playerCursor = this.GetComponent<PlayerCursor>();
            }

            if(this._playerUI == null) {
                if(this.GetComponent<PlayerUI>() == null)
                    this._playerUI = this.gameObject.AddComponent<PlayerUI>();
                else
                    this._playerUI = this.GetComponent<PlayerUI>();
            }

            if(this._playerSound == null) {
                if(this.GetComponent<PlayerSound>() == null)
                    this._playerSound = this.gameObject.AddComponent<PlayerSound>();
                else
                    this._playerSound = this.GetComponent<PlayerSound>();
            }

            if(this._playerCamera == null) {
                if(this.GetComponent<PlayerCamera>() == null)
                    this._playerCamera = this.gameObject.AddComponent<PlayerCamera>();
                else
                    this._playerCamera = this.GetComponent<PlayerCamera>();
            }

            if(this._playerSelect == null) {
                if(this.GetComponent<PlayerSelect>() == null)
                    this._playerSelect = this.gameObject.AddComponent<PlayerSelect>();
                else
                    this._playerSelect = this.GetComponent<PlayerSelect>();
            }

            this._playerCursor.ChangeState(Enum.CursorState.DEFAULT);

            this._playerCamera.Init(this, spawnLocation);

            this._playerUI.Setup();
            this._playerUI.Init(this);

            this._playerSound.Init(this._playerCamera.MainCameraTransform.GetComponent<AudioSource>());

            this._playerSelect.Init(this);
            this._playerSelect.CurrentState = Enum.SelectionState.FREE;

            base.Setup(spawnLocation, id);
        }

        public override void UpdatePlayer() {
            if(Manager.GameManager.instance.PlayerInView == this) {
                this._playerCursor.UpdateCursor();
            }
        }

        #endregion
    }
}