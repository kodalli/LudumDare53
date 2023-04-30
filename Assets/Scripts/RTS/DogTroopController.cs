using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogTroopController : MonoBehaviour, IUnitRts, IStealPackage
{
    [SerializeField] private Animator anim;
    [SerializeField] private AnimatorOverrideController overrideController;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 5f;
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
    private readonly Collider2D[] m_CacheArr = new Collider2D[1];
    private SpriteRenderer m_SpriteRenderer;
    private RuntimeAnimatorController m_OriginalController;
    private bool m_IsHoldingPackage;

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
            Flip();
            anim.SetBool(IsMoving, true);
        }
        else
        {
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

    private void PickUpPackage()
    {
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
                PickUpPackage();
                break;
            }
        }
    }

    private void CheckForPackageDropOff()
    {
        var size = Physics2D.OverlapCircleNonAlloc(transform.position, radius, m_CacheArr, packageDropOffLayer);
        if (size > 0)
        {
            Debug.Log("Dropped Package");
            DropPackage();
        }
    }

    private void Move()
    {
        if (Vector3.Distance(transform.position, m_Destination) < 0.005f)
        {
            m_ReachedDestination = true;
            rb.velocity = Vector2.zero;
        }

        var towards = Vector3.MoveTowards(transform.position, m_Destination, speed * Time.deltaTime);
        rb.MovePosition(towards);
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
        rb.velocity = Vector2.zero;
        m_Destination = destination;
    }

    private void Flip()
    {
        switch (m_FacingRight)
        {
            case true when transform.position.x > m_Destination.x:
                transform.Rotate(0f, -180f, 0f);
                m_FacingRight = false;
                break;
            case false when transform.position.x < m_Destination.x:
                transform.Rotate(0f, -180f, 0f);
                m_FacingRight = true;
                break;
        }
    }

}

