using UnityEngine;
using UnityEngine.InputSystem;

namespace Animalis.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerStatsRuntime))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private PlayerStatsRuntime stats;

        private Vector2 _input;

        private void Awake()
        {
            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
            }

            if (stats == null)
            {
                stats = GetComponent<PlayerStatsRuntime>();
            }
        }

        private void Update()
        {
            _input = ReadMovementInput();
        }

        private void FixedUpdate()
        {
            if (stats == null || !stats.IsAlive)
            {
                body.linearVelocity = Vector2.zero;
                return;
            }

            body.linearVelocity = _input * stats.MoveSpeed;
        }

        private static Vector2 ReadMovementInput()
        {
            Vector2 keyboardInput = Vector2.zero;
            Keyboard keyboard = Keyboard.current;

            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                {
                    keyboardInput.x -= 1f;
                }

                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                {
                    keyboardInput.x += 1f;
                }

                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                {
                    keyboardInput.y -= 1f;
                }

                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                {
                    keyboardInput.y += 1f;
                }
            }

            if (keyboardInput.sqrMagnitude > 1f)
            {
                keyboardInput.Normalize();
            }

            Gamepad gamepad = Gamepad.current;
            Vector2 gamepadInput = gamepad != null ? gamepad.leftStick.ReadValue() : Vector2.zero;

            return gamepadInput.sqrMagnitude > keyboardInput.sqrMagnitude ? gamepadInput : keyboardInput;
        }

        private void Reset()
        {
            body = GetComponent<Rigidbody2D>();
            stats = GetComponent<PlayerStatsRuntime>();
        }
    }
}
