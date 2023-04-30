using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PorchPirateController : MonoBehaviour
{
    [SerializeField] private Transform chevalPackageBaseLocation;
    [SerializeField] private float speed;
    [SerializeField] private float searchRadius;
    [SerializeField] private bool isHoldingPackage;
    [SerializeField] private Transform currentTarget;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float searchCooldown = 2f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] float raycastDistance = 2f;
    [SerializeField] LayerMask obstacleLayer;
    private Collider2D[] nonAlloc = new Collider2D[1];
    private float m_SearchCooldownTimer;
    private bool m_FacingRight = true;

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
    }

    private void FixedUpdate()
    {
        if (isHoldingPackage)
        {
            return;
        }

        if (m_SearchCooldownTimer < 0 && !isHoldingPackage)
        {
            currentTarget = FindClosestPackage();
            m_SearchCooldownTimer = searchCooldown;
        }

        MoveToTarget();
        Flip();

        m_SearchCooldownTimer -= Time.fixedDeltaTime;
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

    private void MoveToTarget()
    {
        // Collision Avoidance
        Vector2 directionToTarget = (currentTarget.position - transform.position).normalized;
        Vector2 finalDirection = directionToTarget;
        // if (rb.velocity.magnitude < 0.01f)
        // {
        //     finalDirection = -finalDirection;
        // }

        float[] angles = new float[] { -55, -30, -15, 0, 15, 30, 55 };
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

            // Debug lines to visualize rays in the Scene view
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

        if (Vector2.Distance(currentTarget.position, transform.position) < 0.05f)
        {
            // TODO: collect package and run away
            isHoldingPackage = true;
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
}
