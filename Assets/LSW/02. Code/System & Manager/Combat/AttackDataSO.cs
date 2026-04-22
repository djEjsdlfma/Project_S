using System;
using UnityEngine;

namespace LSW._02._Code.System___Manager.Combat
{
    [CreateAssetMenu(fileName = "Attack data", menuName = "SO/Attack data", order = 15)]
    public class AttackDataSO : ScriptableObject , ICloneable
    {
        [field: SerializeField] public float Damage { get; set; }
        [field: SerializeField] public Vector2 KnockBackForce { get; set; }
        [field: SerializeField] public bool Critical { get; set; }
        [field: SerializeField] public float StunTime { get; set; }

        public object Clone()
        {
            return Instantiate(this);
        }
    }
}