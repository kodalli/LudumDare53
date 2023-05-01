using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PorchPirateController : MonoBehaviour, IDamageable
{
    public Transform chevalPackageBaseLocation;
    public Transform escapeLocation;
    [SerializeField] private float searchRadius;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float searchCooldown = 2f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] GameObject packagePrefabToDrop;
    [SerializeField] float packageStealRange = 1.5f;
    [SerializeField] private GameObject visiblePackageToCarry;
    [SerializeField] private Material whiteMaterial;
    [SerializeField] private float takeDamageTimer;
    private bool m_IsHoldingPackage;
    private Transform m_CurrentTarget;
    private Collider2D[] m_CacheArr = new Collider2D[1];
    private float m_SearchCooldownTimer;
    private bool m_FacingRight = true;
    private float m_Health = 100f;
    private NavMeshAgent m_Agent;
    private SpriteRenderer m_SpriteRenderer;
    private Material m_DefaultMaterial;

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
        m_CurrentTarget = chevalPackageBaseLocation;
        visiblePackageToCarry.SetActive(false);
        m_DefaultMaterial = m_SpriteRenderer.material;
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
        Utils.Flip(ref m_FacingRight, transform, m_CurrentTarget.position);

        if (m_SearchCooldownTimer < 0)
        {
            return;
        }

        if (m_IsHoldingPackage)
        {
            // currentTarget = ClosestTarget();
            m_SearchCooldownTimer = searchCooldown;
        }
        else
        {
            m_CurrentTarget = FindClosestPackage();
            m_SearchCooldownTimer = searchCooldown;
        }

    }

    private IEnumerator TakeDamageFlash()
    {
        m_SpriteRenderer.material = whiteMaterial;
        yield return new WaitForSeconds(takeDamageTimer);
        m_SpriteRenderer.material = m_DefaultMaterial;
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
        // bug deleting target too early
        if (m_CurrentTarget == null)
        {
            m_CurrentTarget = escapeLocation;
        }

        m_Agent.SetDestination(m_CurrentTarget.position);

        var distanceToTarget = Vector2.Distance(m_CurrentTarget.position, transform.position);

        // Debug.Log("Distance to target: " + distanceToTarget);

        // Successfully brought back package to escape location
        if (distanceToTarget < 1f)
        {
            if (m_CurrentTarget == escapeLocation)
            {
                Debug.Log("Package Gone!");
                App.GameManager.PackageStolen();
                Kill();
            }
            else
            {
                m_CurrentTarget = escapeLocation;
                m_IsHoldingPackage = true;
            }
        }

        // Steal package if in range and don't have a package already
        if (distanceToTarget < packageStealRange && !m_IsHoldingPackage)
        {
            StealPackage();
        }

    }

    private void StealPackage()
    {
        var stealable = m_CurrentTarget.GetComponentInParent<IStealPackage>();
        if (stealable != null)
        {
            var stolen = stealable.StealPackage();
            if (stolen)
            {
                Debug.Log("Package was stolen!");
            }
        }

        // Standalone package was picked so destroy it after picking it up
        var unit = m_CurrentTarget.GetComponentInParent<IUnitRts>();
        if (unit == null && !m_CurrentTarget.CompareTag("dontDestroy"))
        {
            GameObject.Destroy(m_CurrentTarget.gameObject, 0.2f);
        }

        m_IsHoldingPackage = true;
        visiblePackageToCarry.SetActive(true);
        m_CurrentTarget = escapeLocation;
    }

    private void Kill()
    {
        Destroy(gameObject, 0.1f);
    }

    private void DropPackage()
    {
        if (m_IsHoldingPackage)
        {
            GameObject.Instantiate(packagePrefabToDrop, transform.position, Quaternion.identity);
        }
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(TakeDamageFlash());
        m_Health -= damage;
        if (m_Health <= 0f)
        {
            App.GameManager.AddDogTreat();
            DropPackage();
            Kill();
        }
    }
}
