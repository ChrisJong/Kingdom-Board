﻿namespace Helpers {
    public interface ICanAttack : IHasHealth {
        float minDamage { get; }
        float maxDamage { get; }
        float attackRadius { get; }
        float aoeAttackRadius { get; }
        float resistancePercentage { get; }
        float weaknessPercentage { get; }

        Enum.AttackType resistance { get; }
        Enum.AttackType weakness { get; }
        Enum.AttackType attackType { get; }

        float GetDamage();
        void Attack(IHasHealth target);
        void AttackAOE(IHasHealth target);
    }
}