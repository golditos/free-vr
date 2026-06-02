using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stoppingDistance = 1.5f;
    [SerializeField] private bool flyingEnemy = false;
    [SerializeField] private float flyingHeight = 2.5f;

    [Header("Rotation Fix")]
    [SerializeField] private float rotationYOffset = 0f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float hitCooldown = 0.15f;

    [Header("Death Effect")]
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private Vector3 deathEffectOffset = Vector3.zero;
    [SerializeField] private float deathEffectDestroyDelay = 3f;

    private int currentHealth;
    private float lastHitTime = -999f;
    private bool isDead = false;
    private Rigidbody rb;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            if (flyingEnemy)
            {
                rb.useGravity = false;
            }
        }
    }

    private void Start()
    {
        if (target == null && Camera.main != null)
        {
            target = Camera.main.transform;
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        if (target == null) return;

        MoveTowardsTarget();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetFlyingHeight(float newFlyingHeight)
    {
        flyingHeight = newFlyingHeight;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        if (Time.time - lastHitTime < hitCooldown)
        {
            return;
        }

        lastHitTime = Time.time;

        currentHealth -= damageAmount;

        Debug.Log($"{name} recibió daño. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        SpawnDeathEffect();

        Destroy(gameObject);
    }

    private void SpawnDeathEffect()
    {
        if (deathEffectPrefab == null)
        {
            Debug.LogWarning($"{name}: no tiene Death Effect Prefab asignado.");
            return;
        }

        GameObject effect = Instantiate(
            deathEffectPrefab,
            transform.position + deathEffectOffset,
            Quaternion.identity
        );

        ParticleSystem[] particles = effect.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
        }

        Destroy(effect, deathEffectDestroyDelay);
    }

    private void MoveTowardsTarget()
    {
        Vector3 destination = target.position;

        if (flyingEnemy)
        {
            destination.y = flyingHeight;
        }
        else
        {
            destination.y = transform.position.y;
        }

        Vector3 directionToTarget = destination - transform.position;

        if (directionToTarget.magnitude <= stoppingDistance)
        {
            return;
        }

        Vector3 direction = directionToTarget.normalized;
        Vector3 nextPosition = transform.position + direction * moveSpeed * Time.fixedDeltaTime;

        if (rb != null)
        {
            rb.MovePosition(nextPosition);
        }
        else
        {
            transform.position = nextPosition;
        }

        RotateTowards(direction);
    }

    private void RotateTowards(Vector3 direction)
    {
        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);

        if (flatDirection.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = lookRotation * Quaternion.Euler(0f, rotationYOffset, 0f);
        }
    }
    public void KillInstantly()
    {
        if (isDead) return;

        Die();
    }
}