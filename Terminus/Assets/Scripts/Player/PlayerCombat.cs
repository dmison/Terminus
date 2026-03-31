using UnityEngine;

namespace Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
    
        public Transform attackPoint;
        public float attackRange = 0.5f;
        public LayerMask enemyLayers;
        public int attackDamage = 20;
        public float attackRate = 0.5f;
        private float _nextAttackTime = 0f;

        // Update is called once per frame
        private void Update()
        {
            if (playerInput.Attacking && Time.time >= _nextAttackTime)
            {
                Attack();
            }
        }

        private void Attack()
        {
            _nextAttackTime = Time.time + 1f / attackRate;
            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

            foreach(Collider enemy in hitEnemies)
            {
                enemy.GetComponent<Enemy.Enemy>().TakeDamage(attackDamage);
                
            }

        }

        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null)
                return;

            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
