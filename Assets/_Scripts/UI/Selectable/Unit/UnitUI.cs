namespace UI.Selectable.Unit {
    // NOTE: Use delegates and events to change the worldspace canvus camera of the player when the camera switches to differernt states/players.
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    public class UnitUI : MonoBehaviour {

        public Button attackBTN;
        public Button moveBTN;
        public Button endBTN;

        private void Start() {
            this.attackBTN.onClick.AddListener(this.OnAttackClick);
            this.moveBTN.onClick.AddListener(this.OnMoveClick);
            this.endBTN.onClick.AddListener(this.OnEndClick);
        }

        private void OnAttackClick() {
            Debug.Log("Attack Button Clicked!");
        }

        private void OnMoveClick() {
            Debug.Log("Move Button Clicked!");
        }

        private void OnEndClick() {
            Debug.Log("End Button Clicked!");
        }
    }
}