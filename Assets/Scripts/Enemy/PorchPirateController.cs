using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PorchPirateController : MonoBehaviour, IDamageable
{
    [SerializeField] private Transform chevalPackageBaseLocation;
    [SerializeField] private Transform escapeLocation;
    [SerializeField] private float speed;
    [SerializeField] private float searchRadius;
    [SerializeField] private bool isHoldingPackage;
    [SerializeField] private Transform currentTarget;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float searchCooldown = 2f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] float raycastDistance = 2f;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] GameObject packagePrefabToDrop;
    [SerializeField] float packageStealRange = 1.5f;
    [SerializeField] Transform escapePath;
    private List<Transform> pathToEscape = new List<Transform>();
    private Collider2D[] nonAlloc = new Collider2D[1];
    private float m_SearchCooldownTimer;
    private bool m_FacingRight = true;
    private float m_Health = 100f;

    // Logic
    // Search for package
    // Pathfind to package
    // Get package, subtracts from package total
    // Run off screen
    // if pirate killed drops package, use dog to retrieve

    private void Start()
    {
        m_SearchCooldownTimer = searchCooldown;
        currentTarget = chevalPackageBaseLocation;
        pathToEscape.AddRange(escapePath.GetComponentsInChildren<Transform>());
    }

    private void FixedUpdate()
    {

        m_SearchCooldownTimer -= Time.fixedDeltaTime;
        MoveToTarget();
        Flip();

        if (m_SearchCooldownTimer < 0)
        {
            return;
        }

        if (isHoldingPackage)
        {
            currentTarget = ClosestTarget();
            m_SearchCooldownTimer = searchCooldown;
        }
        else
        {
            currentTarget = FindClosestPackage();
            m_SearchCooldownTimer = searchCooldown;
        }

    }

    private Transform FindClosestPackage()
    {
        var size = Physics2D.OverlapCircleNonAlloc(transform.position, searchRadius, nonAlloc, targetLayerMask);
        if (size > 0)
        {
            return nonAlloc[0].transform;
        }
        return chevalPackageBaseLocation;
    }

    private static float[] Arrange(int n, float a, float b)
    {
        float[] array = new float[n];
        float step = (b - a) / (n - 1f);
        for (int i = 0; i < n; i++)
        {
            array[i] = a + i * step;
        }
        return array;
    }

    private Transform ClosestTarget()
    {
        float minDistance = Mathf.Infinity;
        Transform target = escapeLocation;
        for (var i = 0; i < pathToEscape.Count; ++i)
        {
            var point = pathToEscape[i].position;
            var d1 = Vector2.Distance(point, escapeLocation.position);
            var d2 = Vector2.Distance(point, transform.position);
            if (d1 + d2 < minDistance)
            {
                target = pathToEscape[i];
                minDistance = d1 + d2;
            }
        }
        pathToEscape.Remove(target);
        return target;
    }

    private void MoveToTarget()
    {
        // Collision Avoidance
        Vector2 directionToTarget = (currentTarget.position - transform.position).normalized;
        Vector2 finalDirection = directionToTarget;

        float[] angles = Arrange(8, -70f, 70f);
        float minHitDistance = Mathf.Infinity;
        Vector2 closestHitNormal = Vector2.zero;

        foreach (float angle in angles)
        {
            Vector2 rayDirection = RotateVector2(directionToTarget, angle);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, raycastDistance, obstacleLayer);

            if (hit.collider != null)
            {
                if (hit.distance < minHitDistance)
                {
                    minHitDistance = hit.distance;
                    closestHitNormal = hit.normal;
                }
            }

            Debug.DrawRay(transform.position, rayDirection * raycastDistance, hit.collider == null ? Color.green : Color.red);
        }

        if (minHitDistance < Mathf.Infinity)
        {
            Vector2 slideDirection = new Vector2(closestHitNormal.y, -closestHitNormal.x);
            finalDirection = slideDirection;
        }

        finalDirection.Normalize();
        Vector2 targetPosition = rb.position + finalDirection * speed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);

        var distanceToTarget = Vector2.Distance(currentTarget.position, transform.position);

        // Successfully brought back package to escape location
        if (distanceToTarget < 0.1f && currentTarget == escapeLocation)
        {
            Kill();
        }

        // Steal package if in range and don't have a package already
        if (distanceToTarget < packageStealRange && !isHoldingPackage)
        {
            var stealable = currentTarget.GetComponentInParent<IStealPackage>();
            if (stealable != null)
            {
                var stolen = stealable.StealPackage();
                isHoldingPackage = stolen;
                if (stolen)
                {
                    Debug.Log("Package was stolen!");
                }
            }
            currentTarget = escapeLocation;
        }
    }
    private Vector2 RotateVector2(Vector2 vector, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);

        return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
    }

    private void Flip()
    {
        switch (m_FacingRight)
        {
            case true when transform.position.x > currentTarget.position.x:
                transform.Rotate(0f, -180f, 0f);
                m_FacingRight = false;
                break;
            case false when transform.position.x < currentTarget.position.x:
                transform.Rotate(0f, -180f, 0f);
                m_FacingRight = true;
                break;
        }
    }

    private void Kill()
    {
        Destroy(gameObject, 0.1f);
    }

    private void DropPackage()
    {
        GameObject.Instantiate(packagePrefabToDrop);
    }

    public void TakeDamage(float damage)
    {
        m_Health -= damage;
        if (m_Health <= 0f)
        {
            DropPackage();
            Kill();
        }
    }
}
