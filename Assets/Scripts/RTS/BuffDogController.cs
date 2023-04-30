using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuffDogController : MonoBehaviour, IUnitRts
{
    [SerializeField] private SpriteRenderer selectionCircle;
    [SerializeField] private Color selectionColor;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackCoolDown;
    private SpriteRenderer m_SpriteRenderer;
    private Animator m_Anim;
    private bool m_FacingRight;
    private Vector3 m_Destination;
    private bool m_ReachedDestination;
    private NavMeshAgent m_Agent;
    private readonly int m_AttackTrigger = Animator.StringToHash("Attack");
    private readonly int m_IsMoving = Animator.StringToHash("IsMoving");
    private readonly Collider2D[] m_CacheArr = new Collider2D[5];
    private float m_AttackTimer;
    // Start is called before the first frame update
    void Start()
    {
        m_Anim = GetComponent<Animator>();
        m_Agent = GetComponent<NavMeshAgent>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Agent.updateRotation = false;
        m_FacingRight = true;
        m_ReachedDestination = true;
        SetSelectionColor(Color.black);
        m_AttackTimer = attackCoolDown;
    }

    // Update is called once per frame
    void Update()
    {
        // Some reason navmesh forces the sprite rotated 90 on x axis
        var rot = transform.rotation;
        transform.rotation = Quaternion.Euler(new Vector3(0, rot.y, rot.z));
    }

    void FixedUpdate()
    {

        Flip();
        CheckForEnemy();
        if (!m_ReachedDestination)
        {
            m_Agent.isStopped = false;
            Move();
            m_Anim.SetBool(m_IsMoving, true);
        }
        else
        {
            m_Anim.SetBool(m_IsMoving, false);
            m_Agent.isStopped = true;
        }
    }

    private void CheckForEnemy()
    {
        if (m_AttackTimer > 0f)
        {
            m_AttackTimer -= Time.fixedDeltaTime;
            return;
        }

        m_AttackTimer = attackCoolDown;

        var size = Physics2D.OverlapCircleNonAlloc(transform.position, attackRadius, m_CacheArr, enemyLayer);
        if (size > 0)
        {
            m_Destination = m_CacheArr[0].transform.position;
            m_Anim.SetTrigger(m_AttackTrigger);
        }
        while (size > 0)
        {
            size--;
            var enemy = m_CacheArr[size].GetComponent<IDamageable>();
            enemy.TakeDamage(attackDamage);
        }
    }

    private void Move()
    {
        var distanceToTarget = Vector3.Distance(transform.position, m_Destination);
        if (distanceToTarget < 0.1f)
        {
            m_ReachedDestination = true;
        }
        m_Agent.SetDestination(m_Destination);

        // Debug.Log("Distance to target " + distanceToTarget);
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
    private void Flip()
    {
        switch (m_FacingRight)
        {
            case true when transform.position.x > m_Destination.x:
                m_FacingRight = false;
                m_SpriteRenderer.flipX = true;
                break;
            case false when transform.position.x < m_Destination.x:
                m_SpriteRenderer.flipX = false;
                m_FacingRight = true;
                break;
        }
    }
}
