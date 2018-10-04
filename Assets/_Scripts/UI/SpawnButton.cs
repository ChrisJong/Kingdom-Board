namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    public abstract class SpawnButton : MonoBehaviour {
        [SerializeField] protected Image _imageReady = null;
        [SerializeField] protected Image _imageReadyDown = null;
        [SerializeField] protected Image _imageReadyHover = null;
        [SerializeField] protected Image _imageQueue = null;
        [SerializeField] protected Image _imageQueueDown = null;
        [SerializeField] protected Image _imageQueueHover = null;

        [SerializeField] protected bool _ready = false;

    }
}