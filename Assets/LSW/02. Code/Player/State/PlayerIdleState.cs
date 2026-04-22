using LSW._02._Code.Entities;
using LSW._02._Code.Player;
using LSW._02._Code.System___Manager.StateMachine;
using UnityEngine;


public class PlayerIdleState : State
{
    private Player _player;
    private Rigidbody2D _rigidbody;
    private Vector2 _moveInput;

    public PlayerIdleState(Entity owner, EntityStat info, StateMachineCompo stateMachine) : base(owner, info, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Animator.PlayClip(UnityEngine.Animator.StringToHash("IDLE"));

        _player ??= Owner as Player;
        _rigidbody ??= Owner.GetComponent<Rigidbody2D>();
        if (_player != null && _player.inputCompo != null)
            _moveInput = _player.inputCompo.CurrentMoveInput;
        else
            _moveInput = Vector2.zero;

        if (_player != null && _player.inputCompo != null)
        {
            _player.inputCompo.OnMovementAction += HandleMoveInput;
            _player.inputCompo.OnJumpingAction += HandleJumpInput;
        }
    }

    public override void Exit()
    {
        base.Exit();

        if (_player != null && _player.inputCompo != null)
        {
            _player.inputCompo.OnMovementAction -= HandleMoveInput;
            _player.inputCompo.OnJumpingAction -= HandleJumpInput;
        }
    }
    
    public override void UpdateState()
    {
        base.UpdateState();

        if (Mathf.Abs(_moveInput.x) > 0.01f)
            StateMachine.TransitionState("PlayerMoveState");
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        if (_rigidbody == null)
            return;

        _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
    }

    private void HandleMoveInput(Vector2 moveInput)
    {
        _moveInput = moveInput;
    }
    
    private void HandleJumpInput()
    {
        if (_rigidbody != null && Owner.IsGround)
        {
            StateMachine.TransitionState("PlayerJumpState");
        }
    }
}
