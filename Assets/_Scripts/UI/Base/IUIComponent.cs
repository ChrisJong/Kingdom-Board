namespace KingdomBoard.UI {

    using Player;

    public interface IUIComponent {

        bool IsActive { get; set; }
        Player Controller { get; set; }

        void Setup();
        void Init(Player controller);
        void Return();

        void Display();
        void Hide();
        void OnEnter();
        void OnEnter(Player controller);
        void OnExit();
        void ResetUI();
        void UpdateUI();
    }
}