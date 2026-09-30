using UnityEngine;

public class CatFollower : MonoBehaviour
{
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private float stopDistance = 1f;

    private Transform target;

    public void StartFollowing(Transform player)
    {
        target = player;
    }

    public void StopFollowing()
    {
        target = null;
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance > stopDistance)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                followSpeed * Time.deltaTime
            );
        }
    }
}