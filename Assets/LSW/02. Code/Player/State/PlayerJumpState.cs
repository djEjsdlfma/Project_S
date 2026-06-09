using LSW._02._Code.Entities;
using LSW._02._Code.Player;
using LSW._02._Code.System___Manager.StateMachine;
using UnityEngine;

public class PlayerJumpState : State
{
    private const float LandingCheckDelay = 0.08f;

    private Player _player;
    private Rigidbody2D _rigidbody;
    private Vector2 _moveInput;
    private bool _hasLeftGround;
    private float _elapsedAfterEnter;

    public PlayerJumpState(Entity owner, EntityStat info, StateMachineCompo stateMachine) : base(owner, info,
        stateMachine)
    {
        _player = owner as Player;
        _rigidbody = owner?.Rig;
        _hasLeftGround = false;
        _elapsedAfterEnter = 0f;
    }

    public override void Enter()
    {
        base.Enter();

        Animator.PlayClip(UnityEngine.Animator.StringToHash("JUMP"));
        
        if (_rigidbody != null)
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, Info.jumpPower);
        Owner.SetVerticalVelocity(Info.jumpPower);
        
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

        _elapsedAfterEnter += Time.deltaTime;
        Owner.SetVerticalVelocity(_rigidbody.linearVelocity.y);

        if (!_hasLeftGround && (Owner.GetVerticalVelocity() > 0.01f || !Owner.IsGround))
            _hasLeftGround = true;

        if (_hasLeftGround && !Owner.IsGround && Owner.GetVerticalVelocity() < -0.001f)
        {
            StateMachine.TransitionState("PlayerFallingState");
            return;
        }

        if (_elapsedAfterEnter < LandingCheckDelay)
            return;

        if (_hasLeftGround && Owner.IsGround && Owner.GetVerticalVelocity() <= 0.01f)
        {
            if (Mathf.Abs(_moveInput.x) > 0.01f)
                StateMachine.TransitionState("PlayerMoveState");
            else
                StateMachine.TransitionState("PlayerIdleState");

            return;
        }

        HandleJump();
    }

    private void HandleJump()
    {
        _rigidbody.linearVelocity = new Vector2(_moveInput.x * Info.moveSpeed * _player.GetTrapSpeedMultiplier(), _rigidbody.linearVelocity.y);
    }

    private void HandleMoveInput(Vector2 moveInput)
    {
        _moveInput = moveInput;
        
        if (Mathf.Abs(_moveInput.x) > 0.01f)
        {
            Owner.SetFlip(_moveInput.x < 0);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        if (_player != null && _player.InputCompo != null)
            _player.InputCompo.OnMovementAction -= HandleMoveInput;
    }
}