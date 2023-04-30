using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    private Collider2D[] nonAlloc = new Collider2D[1];
    private float m_SearchCooldownTimer;
    private bool m_FacingRight = true;
    private float m_Health = 100f;
    private NavMeshAgent agent;

    // Logic
    // Search for package
    // Pathfind to package
    // Get package, subtracts from package total
    // Run off screen
    // if pirate killed drops package, use dog to retrieve

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        m_SearchCooldownTimer = searchCooldown;
        currentTarget = chevalPackageBaseLocation;
    }

    private void Update()
    {
        var rot = transform.rotation;
        transform.rotation = Quaternion.Euler(new Vector3(0, rot.y, rot.z));
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
            // currentTarget = ClosestTarget();
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

    private void MoveToTarget()
    {
        agent.SetDestination(currentTarget.position);

        var distanceToTarget = Vector2.Distance(currentTarget.position, transform.position);

        Debug.Log("Distance to target: " + distanceToTarget);

        // Successfully brought back package to escape location
        if (distanceToTarget < 1f)
        {
            if (currentTarget == escapeLocation)
            {
                Kill();
            }
            else
            {
                currentTarget = escapeLocation;
                isHoldingPackage = true;
            }
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
