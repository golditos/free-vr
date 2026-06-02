using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 1;

    [Header("After Hit")]
    [SerializeField] private bool destroyEnemyAfterHit = true;

    private bool hasHitPlayer = false;

    private void OnTriggerEnter(Collider other)
    {
        TryDamagePlayer(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryDamagePlayer(collision.gameObject);
    }

    private void TryDamagePlayer(GameObject other)
    {
        if (hasHitPlayer) return;

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null) return;

        hasHitPlayer = true;

        playerHealth.TakeDamage(damage);

        if (destroyEnemyAfterHit)
        {
            EnemyAI enemyAI = GetComponentInParent<EnemyAI>();

            if (enemyAI != null)
            {
                enemyAI.KillInstantly();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}