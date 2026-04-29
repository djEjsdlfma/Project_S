using LSW._02._Code.Entities;
using LSW._02._Code.Player;
using LSW._02._Code.System___Manager.StateMachine;
using UnityEngine;

public class PlayerIdleState : State
{
    private Player _player;
    private Rigidbody2D _rigidbody;
    private Vector2 _moveInput;

    public PlayerIdleState(Entity owner, EntityStat info, StateMachineCompo stateMachine) : base(owner, info,
        stateMachine)
    {
        _player = owner as Player;
        _rigidbody = owner.GetComponent<Rigidbody2D>();
    }

    public override void Enter()
    {
        base.Enter();
        Animator.PlayClip(UnityEngine.Animator.StringToHash("IDLE"));
        
        if (_player != null && _player.InputCompo != null)
            _moveInput = _player.InputCompo.CurrentMoveInput;
        else
            _moveInput = Vector2.zero;

        if (_player != null && _player.InputCompo != null)
        {
            _player.InputCompo.OnMovementAction += HandleMoveInput;
            _player.InputCompo.OnJumpingAction += HandleJumpInput;
        }
        else
        {
            if(_player == null)
                Debug.LogError("_player is Null");
            else if(_player.InputCompo == null)
                Debug.LogError("_player.InputCompo is null");
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
        
        if (Mathf.Abs(_moveInput.x) > 0.01f)
        {
            Owner.SetFlip(_moveInput.x < 0);
        }
        
        Debug.Log($"{Mathf.Abs(_moveInput.x) > 0.01f}, {_moveInput.x < 0}");
    }
    
    private void HandleJumpInput()
    {
        if (_rigidbody != null && Owner.IsGround)
        {
            StateMachine.TransitionState("PlayerJumpState");
        }
    }
}
