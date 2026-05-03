using UI;
using UnityEngine;

namespace Player
{
    public enum PainTypes
    {
        Hit,
        HeadHurts
    }

    public class PlayerHealth : MonoBehaviour
    {
        private static readonly int IsDead = Animator.StringToHash("isDead");
        private static readonly int GotHitTrigger = Animator.StringToHash("gotHit");

        private PlayerMovement playerMovement;

        [SerializeField] private Animator animator;

        [SerializeField] private int maxHealth = 25;
        private int _currentHealth;

        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        private void Start()
        {
            CurrentHealth = maxHealth;
        }

        private void Update()
        {

        }

        public int MaxHealth
        {
            get => maxHealth;
            set
            {
                maxHealth = value;
                _currentHealth = Mathf.Clamp(value, 0, maxHealth);
            }
        }

        public int CurrentHealth
        {
            get => _currentHealth;
            set
            {
                _currentHealth = Mathf.Clamp(value, 0, maxHealth);
                DeathCheck();
            }
        }

        private void DeathCheck()
        {
            if (_currentHealth <= 0)
            {
                animator.SetBool(IsDead, true);
                GetComponentInChildren<SceneSelectorMenu>().ShowMenu(true);
            }
        }

        public void TakeDamage(int amount, PainTypes painType = PainTypes.Hit)
        {
            ImmuneCheck(amount, painType);

            if (_currentHealth <= 0) return;

        }

        public void ImmuneCheck(int amount, PainTypes painType = PainTypes.Hit)
        {
            if (playerMovement.isImmune == true)
            {
                Debug.Log("Im Immune");
                CurrentHealth -= 0;
                return;
            }
            else
            {
                CurrentHealth -= amount;


                switch (painType)
                {
                    case PainTypes.Hit:
                        {
                            animator.SetTrigger(GotHitTrigger);
                            break;
                        }
                }
            }

        }


        public void Heal(int amount)
        {
            CurrentHealth += amount;
        }
    }
}

