using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    public CharacterController controller;
    public Animator animator;
    public Transform camera;
    public Transform groundCheck;
    public LayerMask groundMask;
    Vector3 velocity;
 
    public float speed = 6.0f;
    public float turnSmoothing = 0.1f;
    public float gravity = -9.81f;
    float turnSmoothVelocity;
    public float groundDistance = 0.4f;
    public float jumpHeight = 4.0f;

    //Combat Varials
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 20;
    public float attackRate = 2f;
    float nextAttackTime = 0f;


    bool isGrounded;
    

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // checks if the ground check on the player has hit the ground or not
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // allows the player to move around
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // handles turning and moving/rotating the character model
        if(direction.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothing);
            transform.rotation = Quaternion.Euler(0f, angle, 0f); 

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        //checks if player is grounded or not and resets gravity
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // checks if the player is grounded and allows the player to jump if the jump button is pressed
        if (Input.GetKeyDown(KeyCode.JoystickButton0) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            Debug.Log("Jumping");
        }

      

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton2))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        animator.SetFloat("Speed", Mathf.Abs(direction.magnitude));

    }

    void Attack()
    {
        animator.SetTrigger("Attack");

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
