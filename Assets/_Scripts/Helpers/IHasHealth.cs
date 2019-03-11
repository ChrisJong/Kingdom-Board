﻿namespace KingdomBoard.Helpers {

    using UnityEngine;

    using Player;
    using UI;

    public interface IHasHealth : IEntity {
        float MaxHealth { get; }
        float CurrentHealth { get; }
        float MaxEnergy { get; }
        float CurrentEnergy { get; }

        bool IsDead { get; }
        float LastAttacked { get; }
        IHasHealth LastAttacker { get; set; }
        Player Controller { get; }
        UIBase uiBase { get; }

        void Init(Player contoller);

        bool AddHealth(float amount);
        bool RemoveHealth(float amount);
        bool ReceiveDamage(float damage, IHasHealth enemy);
        bool ReceiveDamage(float damage, IHasHealth enemy, Vector3 origin);
        bool UseEnergy(float amount);
        bool IsAlly(IHasHealth other);
        bool IsEnemy(IHasHealth other);
    }
}