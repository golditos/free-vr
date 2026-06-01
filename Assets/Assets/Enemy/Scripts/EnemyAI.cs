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

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;
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
        if (target == null) return;

        MoveTowardsTarget();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
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
    public void SetFlyingHeight(float newFlyingHeight)
    {
        flyingHeight = newFlyingHeight;
    }
    private void RotateTowards(Vector3 direction)
    {
        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);

        if (flatDirection.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(flatDirection);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}