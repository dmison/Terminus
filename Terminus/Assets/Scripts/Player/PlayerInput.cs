using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInput: MonoBehaviour
    {
        [SerializeField] private InputActionAsset playerControls;
        private InputAction _moveAction;
        private InputAction _pauseAction;

        private bool _paused;

        public Vector2 MoveVector { get; private set; }

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            _moveAction = playerControls.FindActionMap("Player").FindAction("Move");
            _moveAction.performed += ctx => MoveVector = ctx.ReadValue<Vector2>();
            _moveAction.canceled += _ => MoveVector = Vector2.zero;
            
            _pauseAction = playerControls.FindActionMap("Player").FindAction("Pause");
            _pauseAction.performed += _ =>
            {
                _paused = !_paused;
                Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = _paused;
                Time.timeScale = _paused ? 0f : 1f;
            };
        }
    }
}