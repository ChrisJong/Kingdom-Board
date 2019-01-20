namespace UI {

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    using TMPro;

    using Manager;
    using Player;

    public class PlayerEndButton : MonoBehaviour, IPointerUpHandler {

        #region VARIABLE

        [SerializeField] private PlayerUI _playerUI;

        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        #endregion

        #region UNITY

        public void OnPointerUp(PointerEventData eventData) {

            if(this._playerUI.Controller.playerSelect.CurrentState == Enum.SelectionState.FREE || this._playerUI.Controller.playerSelect.CurrentState == Enum.SelectionState.STANDBY)
                this._playerUI.EndTurn();
        }
        #endregion

        #region CLASS

        public void Init(PlayerUI playerUI) {
            this._playerUI = playerUI;

            if(this._button == null)
                this._button = this.transform.GetComponent<Button>() as Button;

            if(this._image == null)
                this._image = this.transform.GetComponent<Image>() as Image;

            if(this._text == null)
                this._text = this.transform.Find("Text").GetComponent<TextMeshProUGUI>() as TextMeshProUGUI;

        }

        public void UpdateButton() {

            float time = GameManager.instance.Countdown;

            this._text.text = Mathf.Round(time).ToString();
        } 

        #endregion
    }
}