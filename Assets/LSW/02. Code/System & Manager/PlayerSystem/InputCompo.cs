
using System;
using LSW._02._Code.Entity;
using LSW._02._Code.Player;
using UnityEngine;

namespace LSW._02._Code.System___Manager.PlayerSystem
{
    public class InputCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private InputReader inputReader;
        
        public Vector2 CurrentMoveInput { get; private set; }
        
        private Player.Player _player;
        private bool _isSubscribed;
        
        public event Action<Vector2> OnMovementAction; 
        public event Action OnJumpingAction; 
        
        public void Initialize(Entities.Entity entity)
        {
            _player = entity as Player.Player;
            if (_player == null)
            {
                Debug.LogError($"Entity {entity.name} is not a Player");
            }

            SubscribeInputEvents();
        }

        private void OnEnable()
        {
            SubscribeInputEvents();
        }

        private void SubscribeInputEvents()
        {
            if (_isSubscribed || inputReader == null)
                return;

            inputReader.OnMovementInput += HandleMovementInput;
            inputReader.OnJumpingInput += HandleJumpInput;
            _isSubscribed = true;
        }

        private void HandleMovementInput(Vector2 movement)
        {
            CurrentMoveInput = movement;
            OnMovementAction?.Invoke(movement);
        }

        private void HandleJumpInput()
        {
            OnJumpingAction?.Invoke();
        }

        private void OnDisable()
        {
            CurrentMoveInput = Vector2.zero;
            if (!_isSubscribed || inputReader == null)
                return;

            inputReader.OnMovementInput -= HandleMovementInput;
            inputReader.OnJumpingInput -= HandleJumpInput;
            _isSubscribed = false;
        }
    }
}