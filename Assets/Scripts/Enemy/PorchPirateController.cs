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
    [SerializeField] GameObject packagePrefabToDrop;
    [SerializeField] float packageStealRange = 1.5f;
    private Collider2D[] m_CacheArr = new Collider2D[1];
    private float m_SearchCooldownTimer;
    private bool m_FacingRight = true;
    private float m_Health = 100f;
    private NavMeshAgent m_Agent;
    private SpriteRenderer m_SpriteRenderer;

    // Logic
    // Search for package
    // Pathfind to package
    // Get package, subtracts from package total
    // Run off screen
    // if pirate killed drops package, use dog to retrieve

    private void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Agent = GetComponent<NavMeshAgent>();
        // Sprite will rotate on z axis of you keep this true
        m_Agent.updateRotation = false;

        m_SearchCooldownTimer = searchCooldown;
        currentTarget = chevalPackageBaseLocation;
    }

    private void Update()
    {
        // Some reason navmesh forces the sprite rotated 90 on x axis
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
        var size = Physics2D.OverlapCircleNonAlloc(transform.position, searchRadius, m_CacheArr, targetLayerMask);
        if (size > 0)
        {
            return m_CacheArr[0].transform;
        }
        return chevalPackageBaseLocation;
    }

    private void MoveToTarget()
    {
        m_Agent.SetDestination(currentTarget.position);

        var distanceToTarget = Vector2.Distance(currentTarget.position, transform.position);

        // Debug.Log("Distance to target: " + distanceToTarget);

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
                m_FacingRight = false;
                m_SpriteRenderer.flipX = true;
                break;
            case false when transform.position.x < currentTarget.position.x:
                m_SpriteRenderer.flipX = false;
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
