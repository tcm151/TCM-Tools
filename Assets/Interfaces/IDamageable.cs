
namespace TCM.Interfaces
{
    public interface IDamageable
    {
        UnityEngine.Vector3 position { get; }
        void TakeDamage(float amount, float knockback = 0, string origin = "null");
    }
}