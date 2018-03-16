namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Player;

    public class PlayerUI : MonoBehaviour {
        private Player _p;

        private GameObject _ui;
        private Canvas _uiCanvas;
        private Text _uiText;
        private Button _endBtn;

        #region UNITY
        private void Awake() {
            this._ui = this.gameObject;
            this._uiCanvas = this._ui.GetComponent<Canvas>() as Canvas;
            this._uiText = this._ui.transform.Find("Info_TEXT").GetComponentInChildren<Text>() as Text;
            this._endBtn = this._ui.transform.Find("EndTurn_BTN").GetComponent<Button>() as Button;

            this._endBtn.onClick.AddListener(this.EndTurnOnClick);
        }
        #endregion

        #region CLASS
        public void Init(Player player) {
            if(player == null)
                Debug.LogError("Player is missing");
            this._p = player;
        }

        public void UpdateUI() {
            if(this._p.TurnEnded) {
                this._endBtn.gameObject.SetActive(false);
            } else {
                this._endBtn.gameObject.SetActive(true);
            }
        }

        public void DisplayUI() {
            this.gameObject.SetActive(true);
        }

        public void HideUI() {
            this.gameObject.SetActive(false);
        }

        private void EndTurnOnClick() {
            this._p.EndTurn();

            this.UpdateUI();
        }
        #endregion
    }
}