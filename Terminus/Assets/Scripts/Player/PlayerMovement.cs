using System.Collections;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{
    public enum PlayerMovementStates
    {
        Idle,
        Moving,
        Falling,
        Jump,
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
        [SerializeField] private bool canDash = true;
        [SerializeField] private bool isDashing = false;
        [SerializeField] private float dashDistance = 10f;
        [SerializeField] private float dodgeCooldown = 2f;
        [SerializeField] private float dodgeTime = 2f;
        
  

        private const float Gravity = 9.81f;

        private void GroundedCheck()
        {
    
            Grounded = Physics.CheckSphere(groundCheck.position, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
        }

        private void Update()
        {
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

                case PlayerMovementStates.Jump:
                    Jump();
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

            if (playerInput.Dodge && canDash && !isDashing)
            {
                _currentMovementState = PlayerMovementStates.Dodge;
            }

            if (playerInput.Jump && Grounded)
            {
                _currentMovementState= PlayerMovementStates.Jump;
            }
        }

        private void Jump()
        {


            if (Grounded)
            {
                _currentMovementState = PlayerMovementStates.Idle;
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
        }

        private void Dodge()
        {
            Debug.Log("Begin Dodge");
            animator.SetBool(IsDodging, true);
            canDash = false;
            isDashing = true;

            StartCoroutine(PerformDodge());
            
            
            if (!isDashing)
            {
                _currentMovementState = PlayerMovementStates.Idle;
                animator.SetBool(IsDodging, false);
            }

        }

        private IEnumerator PerformDodge()
        {
            Vector3 moveDir = GetMoveDir();

            float elapsedTime = 0;


            while (elapsedTime < dodgeTime)
            {
                if (!Grounded)
                {
                    moveDir.y -= Gravity * Time.deltaTime;
                }

                characterController.Move(moveDir.normalized * dashDistance * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
                Debug.Log("Dodge Occuring");
            }

            isDashing = false;

            yield return new WaitForSeconds(dodgeCooldown);
            canDash = true;
            Debug.Log("Dodge Over");
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
