namespace Helpers {
    public interface ICanAttack : IHasHealth {
        float MinDamage { get; }
        float MaxDamage { get; }
        float AttackRange { get; }
        float ResistancePercentage { get; }
        float WeaknessPercentage { get; }
        bool CanAttack { get; }

        Enum.AttackType resistanceType { get; }
        Enum.AttackType weaknessType { get; }
        Enum.AttackType attackType { get; }

        float GetDamage();
        void Attack();
    }
}