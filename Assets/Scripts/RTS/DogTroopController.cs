using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogTroopController : MonoBehaviour, IUnitRts
{
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float radius = 0.3f;
    [SerializeField] private GameObject selectionCircle;

    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int MoveVelocityX = Animator.StringToHash("MovementVelocityX");
    private static readonly int MoveVelocityY = Animator.StringToHash("MovementVelocityY");
    public bool m_ReachedDestination;
    public Vector3 m_Destination;
    private bool m_FacingRight;
    private readonly Collider2D[] m_CacheArr = new Collider2D[1];
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        ToggleSelection(false);
        anim.SetBool(IsMoving, false);
        m_ReachedDestination = false;
        m_FacingRight = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
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

    private void Move()
    {
        // var size = Physics2D.OverlapCircleNonAlloc(m_Destination, radius, m_CacheArr);
        if (Vector3.Distance(transform.position, m_Destination) < 0.005f)
        {
            m_ReachedDestination = true;
            rb.velocity = Vector2.zero;
        }

        var towards = Vector3.MoveTowards(transform.position, m_Destination, speed * Time.deltaTime);
        rb.MovePosition(towards);
    }

    public void ToggleSelection(bool status)
    {
        if (!status)
        {
            // m_ReachedDestination = true;
            selectionCircle.SetActive(false);
        }
        else
        {
            selectionCircle.SetActive(true);
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

