using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f;
    private Rigidbody rb;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Destroy(gameObject, lifetime);
    }
    
    public void Launch(Vector3 direction, float speed)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    void OnCollisionEnter(Collision col)
    {
        HandleHit(col.gameObject, col.contacts[0].point);
    }
    
    void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject, transform.position);
    }

    private void HandleHit(GameObject hit, Vector3 point)
    {
        EnemyAI enemy = hit.GetComponentInParent<EnemyAI>();
        if (enemy != null)
        {
            enemy.TakeDamage(1);
        }
        Destroy(gameObject);
    }
}
