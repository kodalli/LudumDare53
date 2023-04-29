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
    private bool m_ReachedDestination;
    private Vector3 m_Destination;
    private bool m_FacingRight;
    private readonly Collider2D[] m_CacheArr = new Collider2D[1];

    private void Start()
    {
        ToggleSelection(false);
        anim.SetBool(IsMoving, false);
        m_ReachedDestination = false;
        m_FacingRight = true;
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
    }

    private void Move()
    {
        var size = Physics2D.OverlapCircleNonAlloc(m_Destination, radius, m_CacheArr);
        if (size > 0)
        {
            m_ReachedDestination = true;
        }

        rb.MovePosition(transform.position + m_Destination.normalized * Time.deltaTime * speed);
    }

    public void ToggleSelection(bool status)
    {
        if (!status)
        {
            m_ReachedDestination = true;
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

