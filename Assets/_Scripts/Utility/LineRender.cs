namespace Utility {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    [RequireComponent(typeof(LineRenderer))]
    public abstract class LineRender : MonoBehaviour, ILineRender {

        public LineRenderer LineScript { get; set; }

        public abstract void Init();
        public abstract void Draw();
        public abstract void SetActive(bool state);
    }
}