namespace Helpers {
    public interface ICanAttack : IHasHealth {
        float minDamage { get; }
        float maxDamage { get; }
        float attackRadius { get; }
        float resistancePercentage { get; }
        float weaknessPercentage { get; }
        bool canAttack { get; }
        bool isAttacking { get; set; }

        Enum.AttackType resistanceType { get; }
        Enum.AttackType weaknessType { get; }
        Enum.AttackType attackType { get; }

        float GetDamage();
        void Attack(IHasHealth target);
    }
}