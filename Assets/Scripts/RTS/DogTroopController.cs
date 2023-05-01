using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogTroopController : MonoBehaviour, IUnitRts, IStealPackage
{
    [SerializeField] private Animator anim;
    [SerializeField] private AnimatorOverrideController overrideController;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float radius = 0.3f;
    [SerializeField] private SpriteRenderer selectionCircle;
    [SerializeField] private LayerMask packageLayer;
    [SerializeField] private LayerMask packageDropOffLayer;
    // dummy has package layermask for the pirate to collide with
    [SerializeField] private GameObject dummyPackage;
    [SerializeField] private Color selectionColor;

    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int MoveVelocityX = Animator.StringToHash("MovementVelocityX");
    private static readonly int MoveVelocityY = Animator.StringToHash("MovementVelocityY");
    public bool m_ReachedDestination;
    public Vector3 m_Destination;
    private bool m_FacingRight;
    private readonly Collider2D[] m_CacheArr = new Collider2D[4];
    private SpriteRenderer m_SpriteRenderer;
    private RuntimeAnimatorController m_OriginalController;
    private bool m_IsHoldingPackage;
    private NavMeshAgent m_Agent;

    private void Start()
    {
        ToggleSelection(false);
        anim.SetBool(IsMoving, false);
        m_ReachedDestination = false;
        m_FacingRight = true;
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_OriginalController = anim.runtimeAnimatorController;
        m_IsHoldingPackage = false;
        dummyPackage.SetActive(false);
        SetSelectionColor(Color.black);
        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.updateRotation = false;
    }

    void Update()
    {
        // Some reason navmesh forces the sprite rotated 90 on x axis
        var rot = transform.rotation;
        transform.rotation = Quaternion.Euler(new Vector3(0, rot.y, rot.z));
    }

    private void FixedUpdate()
    {
        if (!m_IsHoldingPackage)
        {
            CheckForPackagePickup();
        }
        else
        {
            CheckForPackageDropOff();
        }

        if (!m_ReachedDestination)
        {
            Move();
            Utils.Flip(ref m_FacingRight, transform, m_Destination);
            m_Agent.isStopped = false;
            anim.SetBool(IsMoving, true);
        }
        else
        {
            m_Agent.isStopped = true;
            anim.SetBool(IsMoving, false);
        }
        var normVelocity = rb.velocity.normalized;
        anim.SetFloat(MoveVelocityX, Mathf.RoundToInt(normVelocity.x));
        anim.SetFloat(MoveVelocityY, Mathf.RoundToInt(normVelocity.y));
    }

    public bool StealPackage()
    {
        if (m_IsHoldingPackage)
        {
            DropPackage();
            return true;
        }
        return false;
    }

    private void PickUpPackage(GameObject obj)
    {
        // package is a drop, destroy it 
        if (obj.CompareTag("packageDrop"))
        {
            GameObject.Destroy(obj, 0.1f);
        }

        dummyPackage.SetActive(true);
        m_IsHoldingPackage = true;
        anim.runtimeAnimatorController = overrideController;
    }

    private void DropPackage()
    {
        dummyPackage.SetActive(false);
        m_IsHoldingPackage = false;
        m_ReachedDestination = true;
        anim.runtimeAnimatorController = m_OriginalController;
    }

    private void CheckForPackagePickup()
    {
        // Only pickup packages that are not attached to a dog unit
        var size = Physics2D.OverlapCircleNonAlloc(transform.position, radius, m_CacheArr, packageLayer);
        while (size > 0)
        {
            size--;
            var obj = m_CacheArr[size];
            if (obj.GetComponentInParent<IUnitRts>() == null)
            {
                PickUpPackage(obj.gameObject);
                break;
            }
        }
    }

    private void CheckForPackageDropOff()
    {
        // This layer is the house 
        var size = Physics2D.OverlapCircleNonAlloc(transform.position, radius, m_CacheArr, packageDropOffLayer);
        if (size > 0)
        {
            Debug.Log("Dropped Package at House");
            App.GameManager.DeliverPackage();
            DropPackage();
        }
    }

    private void Move()
    {
        var distanceToTarget = Vector3.Distance(transform.position, m_Destination);
        // Debug.Log("troop Distance To Target " + distanceToTarget);
        if (distanceToTarget < 1f)
        {
            m_ReachedDestination = true;
        }
        m_Agent.SetDestination(m_Destination);
    }

    private void SetSelectionColor(Color color)
    {
        color.a = 0.65f;
        selectionCircle.color = color;
    }

    public void ToggleSelection(bool status)
    {
        if (!status)
        {
            SetSelectionColor(Color.black);
        }
        else
        {
            SetSelectionColor(selectionColor);
        }
    }

    public void MoveToPosition(Vector2 destination)
    {
        m_ReachedDestination = false;
        m_Destination = destination;
    }

}

