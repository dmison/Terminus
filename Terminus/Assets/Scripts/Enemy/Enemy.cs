using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    //function that makes the enemy take damage when hit
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        //*insert code here to change the enemys animation to taking damage

        if (currentHealth < 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy Dies");

        //*insert code here to change the enemy to a dying animation

        // disables the enemy collider and script killing it
        GetComponent<Collider>().enabled = false;    
        this.enabled = false;
    }

    
}
