namespace Helpers {

    using UnityEngine;

    using Player;
    using UI;

    public interface IHasHealth : IEntity {
        float MaxHealth { get; }
        float CurrentHealth { get; }
        float MaxEnergy { get; }
        float CurrentEnergy { get; }

        bool isDead { get; }
        float lastAttacked { get; }
        IHasHealth lastAttacker { get; set; }
        Player Controller { get; }
        UIBase UIComponent { get; }

        void Init(Player contoller);

        bool AddHealth(float amount);
        bool RemoveHealth(float amount);
        bool ReceiveDamage(float damage, IHasHealth target);
        bool ReceiveDamage(float damage, IHasHealth target, Vector3 origin);
        bool UseEnergy(float amount);
        bool IsAlly(IHasHealth other);
        bool IsEnemy(IHasHealth other);
    }
}