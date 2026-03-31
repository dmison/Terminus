using UnityEngine;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        private int _currentHealth;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _currentHealth = maxHealth;
        }

        //function that makes the enemy take damage when hit
        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;

            //*insert code here to change the enemy animation to taking damage

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            //*insert code here to change the enemy to a dying animation

            // disables the enemy collider and script killing it
            GetComponent<Collider>().enabled = false;    
            this.enabled = false;
        }

    
    }
}
