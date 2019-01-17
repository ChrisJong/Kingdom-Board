namespace UI {

    using Player;

    public interface IUIComponent {

        bool IsActive { get; set; }
        Player Controller { get; set; }

        void Setup();
        void Init(Player controller);
        void DisplayUI();
        void HideUI();
        void OnEnter();
        void OnExit();
        void ResetUI();
        void UpdateUI();
    }
}