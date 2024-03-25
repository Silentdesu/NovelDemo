using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Novel.Input
{
    public interface IInputService
    {
        Vector2 MousePosition { get; }
        event Action onActionPerformedEvent;
        void Enable();
        void Disable();
    }

    public sealed class InputService : IInputService
    {
        public Vector2 MousePosition => _input.Game.Mouse.ReadValue<Vector2>();
        public event Action onActionPerformedEvent;
    
        private ProjectInput _input;
    
        public InputService()
        {
            _input = new ProjectInput();
        }

        void IInputService.Enable()
        {
            _input.Game.Enable();
            _input.Game.Action.performed += OnActionPerformed;
        }

        void IInputService.Disable()
        {
            _input.Game.Disable();
        
            _input.Game.Action.performed -= OnActionPerformed;
        }

        private void OnActionPerformed(InputAction.CallbackContext obj)
        {
            onActionPerformedEvent?.Invoke();
        }
    }
}
