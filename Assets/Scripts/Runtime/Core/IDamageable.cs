namespace Animalis.Core
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        void ApplyDamage(DamageData damageData);
    }
}
