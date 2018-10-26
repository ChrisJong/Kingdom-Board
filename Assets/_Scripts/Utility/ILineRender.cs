namespace Utility {

    using UnityEngine;

    public interface ILineRender {

        LineRenderer LineScript { get; set; }

        void Init();
        void Draw();
        void SetActive(bool state);
    }
}