using UnityEngine;

public class PlayerHitboxFollower : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private float bodyHeight = 1.7f;

    private void LateUpdate()
    {
        if (head == null) return;

        Vector3 position = head.position;
        position.y -= bodyHeight * 0.5f;

        transform.position = position;
        transform.rotation = Quaternion.identity;
    }
}