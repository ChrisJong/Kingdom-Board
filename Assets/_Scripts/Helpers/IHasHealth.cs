namespace Helpers {

    using Player;
    using UI;

    public interface IHasHealth : IEntity {
        float maxHealth { get; }
        float currentHealth { get; }
        float maxEnergy { get; }
        float currentEnergy { get; }

        bool isDead { get; }
        float lastAttacked { get; }
        IHasHealth lastAttacker { get; set; }
        Player controller { get; set; }
        UIBase uiComponent { get; set; }

        bool AddHealth(float amount);
        bool ReceiveDamage(float damage, IHasHealth target);
        bool UseEnergy(float amount);
        bool IsAlly(IHasHealth other);
        bool IsEnemy(IHasHealth other);
    }
}