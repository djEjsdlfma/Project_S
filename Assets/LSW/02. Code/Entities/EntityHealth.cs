using System;
using System.Collections;
using System.Linq;
using LSW._02._Code.System___Manager.Combat;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;


namespace LSW._02._Code.Entity
{
    public class EntityHealth : MonoBehaviour, IEntityComponent
    {
        [field: SerializeField] public float MaxHealth { get; private set; }

        public bool IsInvincible { get; set; }
        public float _currentHealth;
        public float _immortalDamage;
        protected Entities.Entity _owner;

        protected float _stunTime;

        protected bool _isStun;


        public delegate void HealthChange(float currentHealth, float maxHealth, bool calculateBlood);
        public event HealthChange OnHealthChangeEvent;
        public event Action OnDead;
        public event Action OnDeadToNotDropItem;

        public event Action<Entities.Entity> OnHitToAgent;
        
        
        public float AvoidBulletChance {get; set;} = 0f;
        public bool Dead => isDead;

        public event Action DiscountDurablity;
        public event Action HitEvent;

        protected bool isDead = false;
        
        private Rigidbody2D _rb;
        private NavMeshAgent _navAgent;
        // private NavMovement _navMovement;

        private Coroutine _co;
        
        public virtual void Initialize(Entities.Entity entity)
        {
            _owner = entity as Entities.Entity;
            InitializeHealth();
            IsInvincible = false;
            _owner.GetComponentsInChildren<IDamageable>().ToList().ForEach((d) => d.Init(this));
            _rb = _owner.GetComponent<Rigidbody2D>();
            _navAgent = _owner.GetComponent<NavMeshAgent>();
            // if(_navAgent != null)
            //     _navMovement = _owner.GetCompo<NavMovement>();
        }

        private void Update()
        {
            if (_isStun)
            {
                _stunTime -= Time.deltaTime;
                if (_stunTime <= 0)
                {
                    _isStun = false;
                    _stunTime = 0;
                    _owner.StunSet(false);
                }
            }
            
            if (Keyboard.current.ctrlKey.wasPressedThisFrame)
            {
                ApplyDeBuffDamage(20);
            }
        }

        public void InitializeHealth()
        {
            _currentHealth = MaxHealth;
            _immortalDamage = 0;
            isDead = false;
        }
        
        public float GetCurrentHealth()
        {
            return _currentHealth;
        }

        /// <summary>
        /// Transform function to change maximum health at runtime.
        /// </summary>
        /// <param name="newMaxHealth">New maximum health value.</param>
        /// <param name="keepRatio">If true, keeps the current health ratio relative to new max. Otherwise, clamps current health.</param>
        public void TransformMaxHealth(float newMaxHealth, bool keepRatio = true)
        {
            if (newMaxHealth <= 0)
                newMaxHealth = 1f;
            float prevMax = MaxHealth;
            float ratio = prevMax > 0 ? _currentHealth / prevMax : 1f;
            MaxHealth = newMaxHealth;
            _currentHealth = keepRatio ? Mathf.Clamp(MaxHealth * ratio, 0f, MaxHealth) : Mathf.Clamp(_currentHealth, 0f, MaxHealth);
            OnHealthChangeEvent?.Invoke( _currentHealth, MaxHealth, false);
        }

        public virtual void ApplyDeBuffDamage(float damage)
        {
            if (damage > 0)
            {
                float paValue = 0;
                _currentHealth = Mathf.Clamp(_currentHealth - Mathf.Max(damage - paValue, 0), 0, MaxHealth);
            }
            else
            {
                // Healing or negative damage
                _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, MaxHealth);
            }

            OnHealthChangeEvent?.Invoke(_currentHealth, MaxHealth, false);

            if (_currentHealth <= 0)
            {
                isDead = true;
                SetDeath();
            }
        }

        public virtual void ApplyDamage(AttackDataSO attackData, Vector3 hitPoint, Vector3 hitNormal, Entities.Entity dealer)
        {
            if (IsInvincible) 
                return;

            if (isDead)
                return;

            float rand = Random.Range(0f, 1f);
            if (rand < AvoidBulletChance)
                return;


            float damageToApply = attackData.Damage;
            float paValue = attackData.Critical ? 2f : 0;

            if (attackData.Critical)
            {
                DiscountDurablity?.Invoke();
                damageToApply = Mathf.Clamp(damageToApply - paValue, 0, 300);
            }

            _currentHealth = Mathf.Clamp(_currentHealth - damageToApply, 0, MaxHealth);

            OnHitToAgent?.Invoke(dealer);
            HitEvent?.Invoke();

            OnHealthChangeEvent?.Invoke(_currentHealth, MaxHealth, true);

            if (_currentHealth <= 0)
            {
                isDead = true;
                SetDeath();
            }
            else
            {
                if (attackData.KnockBackForce.x != 0)
                {
                    Vector2 pushDir = hitNormal.normalized;

                    if (pushDir == Vector2.zero) 
                        pushDir = Vector2.up;
                    
                    float forceAmount = attackData.KnockBackForce.x * _rb.mass; 
    
                    Vector2 finalForce = pushDir * forceAmount;

                    ApplyKnockback(finalForce);
                }
                
                if (attackData.StunTime > 0)
                {
                    _stunTime += attackData.StunTime;
                    if (!_isStun)
                    {
                        _owner.StunSet(true);
                    }
                    _isStun = true;
                }
            }
            _owner.OnHit?.Invoke(true);
        }

        public void ApplyImmortalDamage(float damage)
        {
            if (IsInvincible) 
                return;

            if (isDead)
                return;
            
            _immortalDamage += damage;
            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, MaxHealth);

            HitEvent?.Invoke();
            OnHealthChangeEvent?.Invoke(_currentHealth, MaxHealth, true);
            
                        
            if (_currentHealth <= 0)
            {
                isDead = true;
                SetDeath();
            }
            
            _owner.OnHit?.Invoke(true);
        }

        public void ApplyDamage(float damage, Entities.Entity dealer)
        {
            if (IsInvincible) 
                return;

            if (isDead)
                return;
            
            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, MaxHealth);

            OnHitToAgent?.Invoke(dealer);
            HitEvent?.Invoke();

            OnHealthChangeEvent?.Invoke(_currentHealth, MaxHealth, true);
            
            if (_currentHealth <= 0)
            {
                isDead = true;
                SetDeath();
            }
            
            _owner.OnHit?.Invoke(true);
        }
        
        public void ApplyKnockback(Vector2 hitPoint , Vector2 hitDirection , float hitForce)
        {
            Vector2 pushDir = hitDirection.normalized;

            if (pushDir == Vector2.zero) 
                pushDir = Vector2.up;

            Vector2 finalForce = pushDir * hitForce;

            ApplyKnockback(finalForce);
        }
        
        private void ApplyKnockback(Vector2 force)
        {
            if (_rb != null)
            {
                if (_co != null)
                {
                    StopCoroutine(_co);
                }
                _co = StartCoroutine(KnockbackRoutine(force));
            }
        }

        private IEnumerator KnockbackRoutine(Vector2 force)
        {
            // _navMovement.isKnockback = true;
            _navAgent.enabled = false; 
    
            _rb.linearVelocity = Vector2.zero;
            _rb.AddForce(force, ForceMode2D.Impulse);

            yield return new WaitForSeconds(0.2f);

            _rb.linearVelocity = Vector2.zero;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position; 
            }

            _navAgent.enabled = true;
            // _navMovement.isKnockback = false;
        }
  
        public void SetDeath(bool sendEvent = true)
        {
            _currentHealth = 0;
            if (sendEvent)
                OnDead?.Invoke();
        }

        /*private void TrySpawnBloodEffects(Vector3 hitPoint, Vector3 hitNormal)
        {
            // Spawn splash at the hit point facing out from the surface
            if (bloodSplashPrefab != null)
            {
                var splashRot = Quaternion.LookRotation(hitNormal.sqrMagnitude > 0.0001f ? hitNormal : Vector3.up);
                LeanPool.Spawn(bloodSplashPrefab, hitPoint, splashRot);
            }

            // Spawn ground decal at ground below hit point if possible
            if (bloodGroundPrefab != null)
            {
                Vector3 origin = hitPoint + Vector3.up * 0.1f;
                Quaternion hit = Quaternion.LookRotation(Vector3.up * hitNormal.y);
                if (Physics.Raycast(origin, Vector3.down, out var rh, 2f, ~0, QueryTriggerInteraction.Ignore))
                {
                    LeanPool.Spawn(bloodGroundPrefab, rh.point + rh.normal * 0.005f, Quaternion.identity);
                }
                else
                {
                    LeanPool.Spawn(bloodGroundPrefab, hitPoint,Quaternion.identity);
                }
            }
        }*/

        public void Heal(float amount)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, MaxHealth - _immortalDamage);
            OnHealthChangeEvent?.Invoke(_currentHealth, MaxHealth - _immortalDamage, false);
        }
        
        public void HealToPer(float soul, float maxPer)
        {
            if (soul <= 0f) return;
            if(soul > maxPer)
                soul = maxPer;
            soul = Mathf.Clamp01(soul);
            float targetHealth = _currentHealth + (MaxHealth - _immortalDamage) * soul;
            _currentHealth = Mathf.Clamp(targetHealth, 0f, MaxHealth - _immortalDamage);
            OnHealthChangeEvent?.Invoke(_currentHealth, MaxHealth - _immortalDamage, false);
        }

        public void ApplyDamageToNoDrop(float damage)
        {
            if (damage > 0)
            {
                float paValue = 0;
                _currentHealth = Mathf.Clamp(_currentHealth - Mathf.Max(damage - paValue, 0), 0, MaxHealth);
            }
            else
            {
                // Healing or negative damage
                _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, MaxHealth);
            }

            OnHealthChangeEvent?.Invoke(_currentHealth, MaxHealth, false);

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                isDead = true;
                OnDeadToNotDropItem?.Invoke();
            }
        }
    }
}