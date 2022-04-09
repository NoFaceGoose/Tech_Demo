using UnityEngine;

public class BossHealth : MonoBehaviour
{

    public int health = 500;

    public GameObject deathEffect;
    public GameObject bossHealthBar;

    public bool isInvulnerable = false;

    public void TakeDamage(int damage)
    {
        if (isInvulnerable)
            return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        GetComponent<Boss>().tree.Stop();
        Destroy(gameObject);
        Destroy(bossHealthBar);
    }
}
