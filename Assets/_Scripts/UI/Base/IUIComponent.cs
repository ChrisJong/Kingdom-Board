namespace UI {

    using UnityEngine;
    using UnityEngine.UI;

    using Helpers;
    using Player;

    public interface IUIComponent {

        bool IsActive { get; set; }

        Player controller { get; set; }

        void Init(Player controller);
        void DisplayUI();
        void HideUI();
        void ResetUI();
        void UpdateUI();
    }
}