
using LSW._02._Code.Entity;
using LSW._02._Code.System___Manager.Combat;
using LSW._02._Code.System___Manager.PlayerSystem;
using UnityEngine;

namespace LSW._02._Code.Player
{
    public class Player : Entities.Entity ,IDamageable
    { 
        [field:SerializeField] public InputCompo InputCompo { get; private set; }
        
        // private readonly float walkSoundTime = 0.5f;
        // private readonly float runSoundTime = 0.35f;
        // private float timer = 100;

        // private readonly float waitingTime = 0.35f;

        // private float waitTiemer = 0;
        
        private bool _isSprinting;
        private float _speedModifier;
        
        private EntityHealth _health;
        
        private const string FootStepSound = "WalkSound";
        
        private float _moveDistance;
        
        protected override void Awake()
        {
            base.Awake();            
            _health = GetCompo<EntityHealth>();
            // _staminaCompo = GetCompo<StaminaSystemCompo>();
        }
        
        public void Init(EntityHealth health)
        {
            _health = health;
        }

        // private void OnEnable()
        // {
        //     inputCompo.SetActive(true);
        // }
        // 
        // private void OnDisable()
        // {
        //     inputCompo.SetActive(false);
        // }
        
        private void HandleSprintEvent(bool enable)
        {
            _isSprinting = enable;
        }
        
        // protected override void Update()
        // {
        //     base.Update();
        //     float moveSpeed = GetCurrentMoveSpeed();
        //     _rigid.linearVelocity = inputCompo.InputVector.normalized *
        //                             moveSpeed;
        //     
        //     if (inputCompo.InputVector.sqrMagnitude > 0.1f * 0.1f)
        //     { 
        //         timer += Time.deltaTime;
        //         
        //         if (!_isSprinting)
        //         {
        //             waitTiemer = 0;
        //             if(timer >= walkSoundTime)
        //             {
        //                 // SoundManager.Instance.PlaySound(FootStepSound);
        //                 timer = 0;
        //             }
        //         }
        //         else
        //         {
        //             if(timer >= runSoundTime)
        //             {
        //                 // SoundManager.Instance.PlaySound(FootStepSound);
        //                 timer = 0;
        //             }
        //         }
        //     }
        //     else
        //     {
        //         waitTiemer += Time.deltaTime;
        //         if (waitTiemer >= waitingTime)
        //         {
        //             waitTiemer = 0;
        //             timer = 100;
        //         }
        //     }
        // }

        // protected override void OnDestroy()
        // {
        //     base.OnDestroy();
        //     inputCompo.OnSprintEvent -= HandleSprintEvent;
        //     inputCompo.OnSwapWeaponEvent -= HandleSwapWeaponEvent;
        // }

        public void ApplyDamage(AttackDataSO attackData, Vector3 hitPoint, Vector3 hitNormal, Entities.Entity dealer)
        {
            _health.ApplyDamage(attackData, hitPoint, hitNormal, dealer);
        }

        public void ApplyDeBuffDamage(EffectType effectType, float buffAmount) { }

        public void AddSpeed(float value)
        {
            _speedModifier += value;
        }

        // private float GetCurrentMoveSpeed()
        // {
        //     if (Stat == null)
        //         return 0f;
        //
        //     bool canSprint = _staminaCompo != null ? _staminaCompo.IsRun() : _isSprinting;
        //     float baseSpeed = canSprint ? Stat.runSpeed : Stat.walkSpeed;
        //     return Mathf.Max(0f, baseSpeed + _speedModifier);
        // }
    }
}
