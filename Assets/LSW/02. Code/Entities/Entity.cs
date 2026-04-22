using System;
using System.Collections.Generic;
using System.Linq;
using LSW._02._Code.Entity;
using UnityEngine;
using UnityEngine.Events;

namespace LSW._02._Code.Entities
{
    public class Entity : MonoBehaviour
    {
        private const float GroundedVerticalVelocity = -2f;
        private readonly ContactPoint2D[] _groundContacts = new ContactPoint2D[8];

        public EntityHealth Health { get; private set; }
        public bool IsDead { get; set; }
        
        // [field:SerializeField] public SoundContainSO SoundContain { get; private set; }
        
        public bool IsGround { get; private set; }
        
        public Rigidbody2D Rig { get; private set; }
        
        public UnityEvent<bool> OnHit;
        public UnityEvent OnDeathEvent;

        public UnityEvent<Entity> DeadEvent;

        protected Dictionary<Type, IEntityComponent> _components;

        protected const string HitName = "HIT";

        [field: SerializeField] public EntityStat stat;
        public bool _isStun { get; protected set; } = false;

        private float _verticalVelocity;
        
        protected virtual void Awake()
        {
            Rig = GetComponent<Rigidbody2D>();
            
            _components = GetComponentsInChildren<IEntityComponent>(true)
                .ToDictionary(compo => compo.GetType());
            
            InitComponents();
            AfterInitializeComponent();
        }

        
        protected virtual void Start()
        { }

        protected virtual void Update()
        {
            CheckGround();
        }

        protected virtual void InitComponents()
        {
            _components.Values.ToList().ForEach(component => component.Initialize(this));
            Health = GetCompo<EntityHealth>();
        }

        protected virtual void AfterInitializeComponent()
        {
            _components.Values.OfType<IEntityAfterInitialize>()
                .ToList().ForEach(compo => compo.AfterInitialize());
            Health.OnDead += HandleAgentDead;
            Health.OnHealthChangeEvent += HandleAgentHit;
            Health.HitEvent += AgentHitAction;
        }

        protected virtual void OnDestroy()
        {
            Health.OnDead -= HandleAgentDead;
            Health.OnHealthChangeEvent -= HandleAgentHit;
            Health.HitEvent += AgentHitAction;
        }

        protected virtual void AgentHitAction()
        {
            // SoundContain?.PlaySound(HitName, transform);
        }

        protected virtual void HandleAgentDead()
        {
            OnDeathEvent?.Invoke();
            DeadEvent?.Invoke(this);
        }

        public void AgentDeadGiveEntity()
        {
            DeadEvent?.Invoke(this);
        }

        protected virtual void HandleAgentHit( float current, float max, bool calculateBlood)
        {
            OnHit?.Invoke(calculateBlood);
        }
        
        public T GetCompo<T>()
        {
            if (_components.TryGetValue(typeof(T), out IEntityComponent component) 
                && component is T compo)
            {
                return compo;
            }

            IEntityComponent findComponent = _components.Values.FirstOrDefault(c => c is T);
            if (findComponent is T findCompo)
                return findCompo;
            Debug.LogError($"{typeof(T).ToString()} Compo is Null");
            return default(T);
        }
        public IEntityComponent GetCompo(Type type)
            => _components.GetValueOrDefault(type);

        public virtual void StunSet(bool b)
        {
            _isStun = b;
        }

        public void SetVerticalVelocity(float velocity)
        {
            _verticalVelocity = velocity;
        }

        public float GetVerticalVelocity()
        {
            return _verticalVelocity;
        }
        
        private void CheckGround()
        {
            if (Rig == null)
            {
                IsGround = false;
                return;
            }

            int contactCount = Rig.GetContacts(_groundContacts);
            IsGround = false;

            for (int i = 0; i < contactCount; i++)
            {
                if (_groundContacts[i].normal.y > 0.5f)
                {
                    IsGround = true;
                    break;
                }
            }
        }
    }
}