using UnityEngine;

namespace Player
{
    
    public enum PlayerMovementStates
    {
        Idle,
        Moving,
        Falling,
        Dodge
    }
    
    public class PlayerMovement : MonoBehaviour
    {
        private PlayerMovementStates _currentMovementState = PlayerMovementStates.Idle;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsFalling = Animator.StringToHash("isFalling");
        private static readonly int IsDodging = Animator.StringToHash("isDodging");

        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform playerCameraTransform;
        
        [SerializeField] private Animator animator;
        
        [field: Header("Turning")]
        [SerializeField] private float turnSmoothing = 0.1f;
        [SerializeField] private float turnSmoothVelocity;
    
        [field: Header("Moving")]
        [SerializeField] private float speed = 6.0f;
    
        public bool Grounded { get; private set; } = true;

        [field: Header("Player Grounded")]
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundedRadius = 0.28f;


        [field: Header("Dodge")]
        [SerializeField] private bool canDodge = true;
        [SerializeField] private float dodgeDistance = 10f;
        [SerializeField] private float dodgeTimeRemaining;
        [SerializeField] private float dodgeTime;
        [SerializeField] private Vector3 dodgeDirection;
        [SerializeField] private float dodgeCooldown;
        [SerializeField] private float dodgeReadyTimer = 0.01f; // Keep at 0, makes a timestamp for dodge cooldown.

        private const float Gravity = 9.81f;

        private void GroundedCheck()
        {
            Grounded = Physics.CheckSphere(groundCheck.position, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
        }
        private void DodgeCheck()
        {
            canDodge = dodgeReadyTimer <= Time.time; // dodgeReadyTimer will be Time.time + [dodgeCooldown] when dodge ends, compare until [dodgeCooldown] time has passed.
        }

        private void Awake()
        {
            dodgeTimeRemaining = dodgeTime;
        }

        private void Update()
        {
            Debug.Log(canDodge);
            DodgeCheck();
            GroundedCheck();
            switch (_currentMovementState)
            {
                case PlayerMovementStates.Moving:
                    Moving();
                    break;

                case PlayerMovementStates.Falling:
                    Falling();
                    break;

                case PlayerMovementStates.Idle:
                    Idle();
                    break;

                case PlayerMovementStates.Dodge:
                    Dodge();
                    break;

                default: Idle();
                    break;
            }
        }

        private void Idle()
        {
            animator.SetFloat(Speed, 0.0f);
            
            // == transitions
            if (!Grounded)
            {
                _currentMovementState = PlayerMovementStates.Falling;
            }
            
            if (Grounded && playerInput.MoveVector != Vector2.zero)
            {
                _currentMovementState = PlayerMovementStates.Moving;
            }

            if (playerInput.Dodge && canDodge && Grounded)
            {
                _currentMovementState = PlayerMovementStates.Dodge;

                animator.SetTrigger("Dodge");
            }
        }

        private void Falling()
        {
            animator.SetBool(IsFalling, true);
            characterController.Move(GetMoveDir().normalized * (speed * Time.deltaTime));
            
            // == transitions
            if (Grounded)
            {
                _currentMovementState = PlayerMovementStates.Idle;
                animator.SetBool(IsFalling, false);
            }

        }

        private void Moving()
        {  
            Vector3 moveDir = GetMoveDir();
            float moveSpeed = GetMoveSpeed();
            animator.SetFloat(Speed, moveSpeed);
            characterController.Move(moveDir.normalized * (speed * moveSpeed * Time.deltaTime));
            
            // == transitions
            if (!Grounded)
            {
                _currentMovementState = PlayerMovementStates.Falling;
            }
            
            if (Grounded && playerInput.MoveVector == Vector2.zero)
            {
                _currentMovementState = PlayerMovementStates.Idle;
            }

            if (playerInput.Dodge && Grounded && canDodge)
            {
                _currentMovementState |= PlayerMovementStates.Dodge;

                animator.SetTrigger("Dodge");
            }
        }

        private void Dodge()
        {
            dodgeTimeRemaining -= Time.deltaTime;

            if (dodgeTimeRemaining <= 0f)
            {
                dodgeReadyTimer = Time.time + dodgeCooldown; // Adds cooldown to Time.time. Sets a 'bookmark' that is [dodgeCooldown] amount of time in the future.
                _currentMovementState = PlayerMovementStates.Idle;
                dodgeTimeRemaining = dodgeTime;  //reset the time remaining for next dodge
                dodgeDirection = Vector3.zero;
                canDodge = false;
                return;
            }

            if (dodgeDirection == Vector3.zero)
            {
                dodgeDirection = GetMoveDir();
            }

            characterController.Move(dodgeDirection.normalized * (dodgeDistance * Time.deltaTime));
        }

        /**
         returns 0 to 1 depending on movement input
         keyboard always returns 1 if movement key is pressed
         gamepad returns between 0 & 1 depending on how far stick is pressed
         */
        private float GetMoveSpeed()
        {
            return Mathf.Abs(playerInput.MoveVector.magnitude);
        }
        
        private Vector3 GetMoveDir()
        {
            // handles turning and moving/rotating the character model
            float targetAngle = Mathf.Atan2(playerInput.MoveVector.x, playerInput.MoveVector.y) * Mathf.Rad2Deg + playerCameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothing);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            if (!Grounded)
            {
                moveDir.y -= Gravity;
            }
            else
            {
                moveDir.y = 0f;
            }

            return moveDir;

        }


    }
}
