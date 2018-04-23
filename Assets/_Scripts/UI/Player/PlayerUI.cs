namespace UI {

    using UnityEngine;
    using UnityEngine.UI;

    using Player;
    using UI;
    using System;

    public class PlayerUI : ScreenSpaceUI {

        #region VARIABLE
        public Text text;
        public Button btnEnd;
        #endregion

        #region UNITY
        private void Awake() {
            this.btnEnd.onClick.AddListener(this.EndTurn);
        }
        #endregion

        #region CLASS
        public void Init(Player player) {
            if(player == null)
                Debug.LogError("Player is missing");
            this.controller = player;
            
        }

        public override void Display() {
            this.isActive = true;
        }

        public override void Hide() {
            this.isActive = false;
        }

        protected override void Reset() {
            throw new NotImplementedException();
        }

        public override void UpdateUI() {
            if(this.controller.turnEnded) {
                this.Hide();
            } else {
                
            }
        }

        private void EndTurn() {
            this.controller.EndTurn();
            this.UpdateUI();
        }
        #endregion
    }
}