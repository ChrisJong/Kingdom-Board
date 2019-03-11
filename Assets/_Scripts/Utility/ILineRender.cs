namespace KingdomBoard.Utility {

    using UnityEngine;

    public interface ILineRender {

        LineRenderer lineRenderer { get; }

        void Init();
        void Draw();
        void SetActive(bool state);
    }
}