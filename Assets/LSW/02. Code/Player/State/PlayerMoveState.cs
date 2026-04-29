using LSW._02._Code.Entities;
using LSW._02._Code.Player;
using LSW._02._Code.System___Manager.StateMachine;
using UnityEngine;

public class PlayerMoveState : State
{
    private Player _player;
    private Rigidbody2D _rigidbody;
    private Vector2 _moveInput;

    public PlayerMoveState(Entity owner, EntityStat info, StateMachineCompo stateMachine) : base(owner, info,
        stateMachine)
    {
        _player = Owner as Player;
        _rigidbody = Owner.GetComponent<Rigidbody2D>();
    }

    public override void Enter()
    {
        base.Enter();
        
        Animator.PlayClip(UnityEngine.Animator.StringToHash("MOVE"));
        
        if (_player != null && _player.InputCompo != null)
            _moveInput = _player.InputCompo.CurrentMoveInput;

        if (_player != null && _player.InputCompo != null)
        {
            _player.InputCompo.OnMovementAction += HandleMoveInput;
            _player.InputCompo.OnJumpingAction += HandleJumpInput;
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        if (_player != null && _player.InputCompo != null)
        {
            _player.InputCompo.OnMovementAction -= HandleMoveInput;
            _player.InputCompo.OnJumpingAction -= HandleJumpInput;
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (!Owner.IsGround && _rigidbody.linearVelocity.y < -0.001f)
        {
            StateMachine.TransitionState("PlayerFallingState");
            return;
        }

        if (Mathf.Abs(_moveInput.x) <= 0.01f)
            StateMachine.TransitionState("PlayerIdleState");
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        if (_rigidbody == null)
            return;

        _rigidbody.linearVelocity = new Vector2(_moveInput.x * Info.moveSpeed, _rigidbody.linearVelocity.y);
    }

    private void HandleMoveInput(Vector2 moveInput)
    {
        _moveInput = moveInput;
        
        if (Mathf.Abs(_moveInput.x) > 0.01f)
        {
            Owner.SetFlip(_moveInput.x < 0);
        }
    }
    
    private void HandleJumpInput()
    {
        if (_rigidbody != null && Owner.IsGround)
        {
            StateMachine.TransitionState("PlayerJumpState");
        }
    }
}