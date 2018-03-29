namespace Helpers {

    using Player;

    public interface IHasHealth : IEntity {
        float maxHealth { get; }
        float currentHealth { get; }
        float maxEnergy { get; }
        float currentEnergy { get; }

        bool isDead { get; }
        float lastAttacked { get; }
        IHasHealth lastAttacker { get; set; }
        Player controller { get; set; }

        bool ReceiveDamage(float damage);
        bool UseEnergy(float amount);
        bool IsAlly(IHasHealth other);
        bool IsEnemy(IHasHealth other);
    }
}