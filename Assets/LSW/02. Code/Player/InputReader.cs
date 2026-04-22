using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LSW._02._Code.Player
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "SO/InputReader", order = 0)]
    public class InputReader : ScriptableObject, Inputs.IPlayerActions
    {
        public event Action<Vector2> OnMovementAction;
        public event Action OnJumpingAction;
        
        public Vector2 CurrentMoveInput { get; private set; }

        private Inputs _input;
        
        private void OnEnable()
        {
            if (_input == null)
            {
                _input = new Inputs();
                EnableInput(true);
            }
            _input.Player.SetCallbacks(this);
        }

        private void OnDisable()
        {
            CurrentMoveInput = Vector2.zero;
            EnableInput(false);
        }

        public void EnableInput(bool enable)
        {
            if (_input != null)
            {
                if(_input.Player.enabled == enable)
                    return;
                
                if(enable)
                    _input.Player.Enable();
                else
                    _input.Player.Disable();
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 move = context.ReadValue<Vector2>();
            CurrentMoveInput = move;
            OnMovementAction?.Invoke(move);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnJumpingAction?.Invoke();
        }
    }
}