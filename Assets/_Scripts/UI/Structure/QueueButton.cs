namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using TMPro;

    using Enum;
    using Structure;

    public class QueueButton : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler {

        #region VARIABLE

        [SerializeField] private Castle _castle;
        [SerializeField] private SpawnQueueType _queueType;

        [SerializeField] private Color _readyColor;
        [SerializeField] private Color _waitingColor = Color.white;

        [SerializeField] private Sprite _iconSprite = null;

        [SerializeField] private RectTransform _rectTransform;

        [SerializeField] private Animation _animation;
        [SerializeField] private Animation _iconAnimation;
        [SerializeField] private Animation _textAnimation;

        [SerializeField] private Button _button;

        [SerializeField] private Image _image;
        [SerializeField] private Image _iconImage;

        [SerializeField] private TextMeshProUGUI _text;

        #endregion

        public RectTransform rectTransfrom { get { return this._rectTransform; } }

        public UnitType typeToSpawn { get { return this._queueType.type; } }

        public bool ready { get { return this._queueType.ready; } set { this._queueType.ready = value; } }

        public uint ID { get { return this._queueType.id; } }

        #region UNITY

        public void OnPointerClick(PointerEventData eventData) {}

        public void OnPointerUp(PointerEventData eventData) {
            if(this._castle.controller.state == PlayerState.DEFENDING)
                return;

            if(this.ready) {
                this.Spawn();
            } else {
                this.Delete();
            }
        }

        public void OnPointerDown(PointerEventData eventData) {}

        #endregion

        #region CLASS

        public void Init(float yPos, Castle castle, SpawnQueueType type, Color unitColor, Sprite unitIcon) {

            Vector3 pos = new Vector3(-390.0f, yPos, 0.0f);

            this._castle = castle;
            this._queueType = type;
            this._readyColor = unitColor;
            this._iconSprite = unitIcon;

            this._rectTransform = this.transform as RectTransform;
            this._rectTransform.anchoredPosition = pos;

            if(this._animation == null)
                this._animation = this.transform.GetComponent<Animation>() as Animation;
            if(this._button == null)
                this._button = this.transform.GetComponent<Button>() as Button;
            if(this._image == null)
                this._image = this.transform.GetComponent<Image>() as Image;

            this._image.color = this._waitingColor;

            if(this._iconImage == null)
                this._iconImage = this.transform.Find("UnitIcon_IMG").GetComponent<Image>() as Image;
            if(this._iconAnimation == null)
                this._iconAnimation = this._iconImage.gameObject.GetComponent<Animation>() as Animation;

            this._iconImage.sprite = this._iconSprite;

            if(this._text == null)
                this._text = this.transform.Find("Unit_Text").GetComponent<TextMeshProUGUI>() as TextMeshProUGUI;
            if(this._textAnimation == null)
                this._textAnimation = this._text.gameObject.GetComponent<Animation>() as Animation;

            this._text.text = this._queueType.type.ToString().ToLower();

            this.PlayCreateAnimation();
        }

        #endregion

        public void Ready() {
            this._image.color = this._readyColor;
        }

        public void Delete() {
            if(!this._queueType.ready)
                this.PlayCancelAnimation();

            //Destroy(this.gameObject);
        }

        public IEnumerator MoveButton(float yPos, float moveSpeed) {

            Vector3 pos = this._rectTransform.anchoredPosition;

            while(pos.y < yPos) {

                pos.y += moveSpeed;

                if(pos.y > yPos)
                    pos.y = yPos;

                this.rectTransfrom.anchoredPosition = pos;

                yield return new WaitForEndOfFrame();
            }

            yield return null;
        }

        private void Spawn() {
            Debug.Log("Spawn");
            //this._castle.SetSpawn(this.id);
        }

        private void Remove() {
            this._castle = null;

            this._queueType.Remove();
            this._queueType = null;

            Destroy(this.gameObject);
        }

        private void FinishCancelAnimation() {
            this._castle.RemoveUnitFromQueue(this._queueType);
            this.Remove();
        }

        private void PlayCreateAnimation() {
            this._animation.Play("ribbonSpawn");
        }

        private void PlayFadeAnimation() {

        }

        private void PlayCancelAnimation() {
            float timer = this._animation.GetClip("ribbonCancel").length;

            this._animation.Play("ribbonCancel");
            this._iconAnimation.Play("ribbonCancel");
            this._textAnimation.Play("ribbonTextCancel");

            this.Invoke("FinishCancelAnimation", timer);
        }

        private void PlaySpawnAnimation() {

        }

        private void PlaySpawnedAnimation() {

        }
    }
}