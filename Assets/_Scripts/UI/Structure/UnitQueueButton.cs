namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Enum;
    using Structure;

    public class UnitQueueButton : MonoBehaviour {

        [SerializeField] private Castle _castle;
        [SerializeField] private CastleUI _castleUI;
        [SerializeField] private SpawnQueueType _queueType;

        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private SpriteState _spriteState = new SpriteState();

        [SerializeField] private UnitType _typeToSpawn;

        [SerializeField] private Sprite _queueSprite;
        [SerializeField] private Sprite _RemoveOverlay;
        [SerializeField] private Sprite _ReadyOverlay;

        [SerializeField] private uint _id = 0;

        public UnitType typeToSpawn { get { return this._typeToSpawn; } }

        public bool ready { get { return this._queueType.ready; } set { this._queueType.ready = value; } }

        public uint id { get { return this._id; } }

        private void ButtonClick() {
            if(this.ready) {
                this.Spawn();
            } else {
                this.Delete();
            }
        }

        private void Init() {
            if(this._button == null)
                this._button = this.GetComponent<Button>() as Button;

            if(this._image == null)
                this._image = this.GetComponent<Image>() as Image;
        }

        public void Init(uint id, UnitType type, Sprite sprite) {
            this.Init();

            this._id = id;

            this._typeToSpawn = type;
            this._queueSprite = sprite;

            this._image.sprite = sprite;

            this._spriteState.highlightedSprite = this._RemoveOverlay;

            if(this._button != null) {
                this._button.spriteState = this._spriteState;
                this._button.onClick.AddListener(this.ButtonClick);
            }

            if(this._image != null)
                this._image.sprite = this._queueSprite;
        }

        public void SetQueueType(SpawnQueueType queue) {
            this._queueType = queue;
        }

        public void Init(uint id, UnitType type, Sprite sprite, Castle castle, CastleUI ui) {
            this.Init(id, type, sprite);

            if(this._castle == null)
                this._castle = castle;

            if(this._castleUI == null)
                this._castleUI = ui;
        }

        public void Ready() {
            this._spriteState.highlightedSprite = this._ReadyOverlay;

            if(this._button != null)
                this._button.spriteState = this._spriteState;
        }

        private void Spawn() {
            // A switch or change needs to happen so you can get a point in the world to spawn the unit.

            this._castle.SetSpawn(this.id);

            Debug.Log("Ready to Spawn: " + _queueType.ToString());
        }

        public void Delete() {
            if(!this._queueType.ready)
                this._castleUI.RemoveFromQueue(this);
            this._button.onClick.RemoveListener(this.ButtonClick);

            Destroy(this.gameObject);
        }

        private void RemoveButton() {
            this._button.onClick.RemoveListener(this.ButtonClick);
        }
    }
}