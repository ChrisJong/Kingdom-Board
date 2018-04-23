namespace UI {

    using UnityEngine;
    using UnityEngine.UI;

    using Helpers;
    using Player;

    public interface IUIComponent {

        Canvas canvas { get; }
        GameObject gameObject { get; }
        Transform transform { get; }
        RectTransform rectTransform { get; }

        Player controller { get; set; }

        bool isActive { get; set; }

        void Display();
        void Hide();
    }
}