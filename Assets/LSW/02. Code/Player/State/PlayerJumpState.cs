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

    public PlayerJumpState(Entity owner, EntityStat info, StateMachineCompo stateMachine) : base(owner, info, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        
        Debug.Log("Entered PlayerJumpState");
        Animator.PlayClip(UnityEngine.Animator.StringToHash("JUMP"));

        _player ??= Owner as Player;
        _rigidbody ??= Owner.Rig;
        _hasLeftGround = false;
        _elapsedAfterEnter = 0f;
        Owner.SetVerticalVelocity(Info.jumpPower);
        
        if (_player != null && _player.inputCompo != null)
            _moveInput = _player.inputCompo.CurrentMoveInput;
        else
            _moveInput = Vector2.zero;

        if (_player != null && _player.inputCompo != null)
            _player.inputCompo.OnMovementAction += HandleMoveInput;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (_player == null || _rigidbody == null)
            return;

        _elapsedAfterEnter += Time.deltaTime;

        if (!_hasLeftGround && (Owner.GetVerticalVelocity() > 0.01f || !Owner.IsGround))
            _hasLeftGround = true;

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
        _rigidbody.linearVelocity = new Vector2(_moveInput.x * Info.moveSpeed, _rigidbody.linearVelocity.y);
    }

    private void HandleMoveInput(Vector2 moveInput)
    {
        _moveInput = moveInput;
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("Exited PlayerJumpState");
        if (_player != null && _player.inputCompo != null)
            _player.inputCompo.OnMovementAction -= HandleMoveInput;
    }
}