using UnityEngine;

namespace Animalis.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerDirectionalAnimator : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private Animator animator;
        [Min(0f)]
        [SerializeField] private float movingThreshold = 0.05f;

        [Header("Animator States")]
        [SerializeField] private string idleState = "FoxIdle";
        [SerializeField] private string walkDownState = "FoxWalkDown";
        [SerializeField] private string walkUpState = "FoxWalkUp";
        [SerializeField] private string walkLeftState = "FoxWalkLeft";
        [SerializeField] private string walkRightState = "FoxWalkRight";

        private string _currentState;

        private void Awake()
        {
            ResolveReferences();
        }

        private void Update()
        {
            if (animator == null || body == null)
            {
                return;
            }

            Vector2 velocity = body.linearVelocity;
            if (velocity.sqrMagnitude <= movingThreshold * movingThreshold)
            {
                PlayState(idleState);
                return;
            }

            string stateName;
            if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
            {
                stateName = velocity.x < 0f ? walkLeftState : walkRightState;
            }
            else
            {
                stateName = velocity.y > 0f ? walkUpState : walkDownState;
            }

            PlayState(stateName);
        }

        private void PlayState(string stateName)
        {
            if (string.IsNullOrWhiteSpace(stateName) || _currentState == stateName)
            {
                return;
            }

            int stateHash = GetStateHash(stateName);
            if (stateHash == 0)
            {
                return;
            }

            animator.Play(stateHash, 0);
            _currentState = stateName;
        }

        private int GetStateHash(string stateName)
        {
            int stateHash = Animator.StringToHash(stateName);
            if (!animator.HasState(0, stateHash))
            {
                stateHash = Animator.StringToHash($"Base Layer.{stateName}");
            }

            return animator.HasState(0, stateHash) ? stateHash : 0;
        }

        private void Reset()
        {
            ResolveReferences();
        }

        private void OnValidate()
        {
            ResolveReferences();
            movingThreshold = Mathf.Max(0f, movingThreshold);
        }

        private void ResolveReferences()
        {
            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
            }

            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>(true);
            }
        }
    }
}
