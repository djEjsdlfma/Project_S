using LSW._02._Code.Entities;
using LSW._02._Code.Player;
using LSW._02._Code.System___Manager.StateMachine;
using UnityEngine;

public class PlayerFallingState : State
{
    private Player _player;
    private Rigidbody2D _rigidbody;
    private Vector2 _moveInput;

    public PlayerFallingState(Entity owner, EntityStat info, StateMachineCompo stateMachine) : base(owner, info,
        stateMachine)
    {
        _player = owner as Player;
        _rigidbody = owner?.Rig;
    }

    public override void Enter()
    {
        base.Enter();

        Animator.PlayClip(UnityEngine.Animator.StringToHash("JUMP"));

        if (_player != null && _player.InputCompo != null)
            _moveInput = _player.InputCompo.CurrentMoveInput;
        else
            _moveInput = Vector2.zero;

        if (_player != null && _player.InputCompo != null)
            _player.InputCompo.OnMovementAction += HandleMoveInput;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (_player == null || _rigidbody == null)
            return;

        Owner.SetVerticalVelocity(_rigidbody.linearVelocity.y);

        if (Owner.IsGround && Owner.GetVerticalVelocity() <= 0.01f)
        {
            if (Mathf.Abs(_moveInput.x) > 0.01f)
                StateMachine.TransitionState("PlayerMoveState");
            else
                StateMachine.TransitionState("PlayerIdleState");

            return;
        }

        HandleFalling();
    }

    private void HandleFalling()
    {
        _rigidbody.linearVelocity = new Vector2(_moveInput.x * Info.moveSpeed, _rigidbody.linearVelocity.y);
    }

    private void HandleMoveInput(Vector2 moveInput)
    {
        _moveInput = moveInput;
    }

    public override void Exit()
    {
        base.Exit();

        if (_player != null && _player.InputCompo != null)
            _player.InputCompo.OnMovementAction -= HandleMoveInput;
    }
}