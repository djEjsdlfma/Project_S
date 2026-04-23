using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LSW._02._Code.Player
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "SO/InputReader", order = 0)]
    public class InputReader : ScriptableObject, Inputs.IPlayerActions
    {
        public event Action<Vector2> OnMovementInput;
        public event Action OnJumpingInput;

        private Inputs _input;
        
        private void OnEnable()
        {
            if (_input == null)
            {
                _input = new Inputs();
                _input.Player.SetCallbacks(this);
            }
            EnableInput(true);
        }

        private void OnDisable()
        {
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
            OnMovementInput?.Invoke(move);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnJumpingInput?.Invoke();
        }
    }
}