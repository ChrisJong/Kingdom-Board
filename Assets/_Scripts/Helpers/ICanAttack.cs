namespace Helpers {
    public interface ICanAttack : IHasHealth {
        float minDamage { get; }
        float maxDamage { get; }
        float attackRadius { get; }
        float aoeAttackRadius { get; }

        float GetDamage();
        void Attack(IHasHealth target);
        void AttackAOE(IHasHealth target);
    }
}