using LSW._02._Code.Entity;
using UnityEngine;
using LSW._02._Code.Entities;

namespace LSW._02._Code.System___Manager.Combat
{
    public interface IDamageable
    {
        public void Init(EntityHealth health);
        public void ApplyDamage(AttackDataSO attackData, Vector3 hitPoint, Vector3 hitNormal,  Entities.Entity dealer);
        public void ApplyDeBuffDamage(EffectType effectType, float buffAmount);
    }

    public enum EffectType
    {
    }
}