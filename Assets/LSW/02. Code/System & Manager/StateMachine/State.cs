using CSI._01Script.Animation;
using LSW._02._Code.Entities;

namespace LSW._02._Code.System___Manager.StateMachine
{
    public abstract class State
    {
        protected Entities.Entity Owner;
        protected EntityStat Info;
        protected StateMachineCompo StateMachine;
        protected EntityAnimator Animator;
    
        protected State(Entities.Entity owner, EntityStat info, StateMachineCompo stateMachine){
            Owner = owner;
            Info = info;
            StateMachine = stateMachine;
            Animator = owner.GetCompo<EntityAnimator>();
        }
    
        public virtual void Enter()
        {
    
        }

        public virtual void Exit()
        {
    
        }

        public virtual void UpdateState()
        {

        }

        public virtual void FixedUpdateState()
        {

        }
    }
}