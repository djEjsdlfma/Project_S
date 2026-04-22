using System;
using LSW._02._Code.Entities;
using LSW._02._Code.Entity;
using UnityEngine;

namespace CSI._01Script.Animation
{
    public class EntityAnimator : MonoBehaviour,IEntityComponent,IEntityAnimator
    {
        private Entity _entity;
        public Animator Animator { get; private set;  }
        public event Action OnAnimationEnd;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            Animator = GetComponent<Animator>();
            Animator.Rebind();

        }

        private void OnEnable()
        {
            if (Animator != null)
            {
                Animator.enabled = true;
                Animator.Rebind();
                Animator.Update(0);
            }
            _entity?.OnDeathEvent.AddListener(HandleDeath);
        }

        private void OnDisable()
        {
            _entity?.OnDeathEvent.RemoveListener(HandleDeath);
            ResetBooleanParams();
            transform.localPosition = Vector3.zero;
        }

        private void HandleDeath()
        {
            // Animator.enabled = false;
        }

        public void PlayClip(int clipHash, int layer = -1, float normalPosition = float.NegativeInfinity)
            => Animator.Play(clipHash, layer, normalPosition);
        
        public void SetParam(AnimParamSO param, bool value) => Animator.SetBool(param.HashValue, value);
        public void SetParam(AnimParamSO param, int value) => Animator.SetInteger(param.HashValue, value);
        public void SetParam(AnimParamSO param, float value) => Animator.SetFloat(param.HashValue, value);
        public void SetParam(AnimParamSO param) => Animator.SetTrigger(param.HashValue);
        

        public void ResetBooleanParams()
        {
            foreach(AnimatorControllerParameter param in Animator.parameters)
                if(param.type == AnimatorControllerParameterType.Bool)
                    Animator.SetBool(param.name, false);
        }

        private void AnimationEndTrigger() => OnAnimationEnd?.Invoke();

        public void SetParam(int stringToHash, bool value)
        {
            Animator.SetBool(stringToHash, value);
        }
    }
}