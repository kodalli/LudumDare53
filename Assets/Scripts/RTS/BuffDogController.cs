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
    [SerializeField] private ParticleSystem particleSystem;
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
    private float m_SeekCoolDown = 0;
    private Transform m_CurrentTarget;
    public AudioClip punchSound;
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

        Utils.Flip(ref m_FacingRight, transform, m_Destination);
        CheckForEnemy();
        SeekEnemy();
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

    private void SeekEnemy()
    {
        if (m_CurrentTarget != null)
        {
            m_Destination = m_CurrentTarget.position;
        }

        if (m_SeekCoolDown < 5f)
        {
            m_SeekCoolDown += Time.fixedDeltaTime;
            return;
        }
        m_SeekCoolDown = 0f;
        var size = Physics2D.OverlapCircleNonAlloc(transform.position, attackRadius * 3f, m_CacheArr, enemyLayer);
        if (size > 0)
        {
            MoveToPosition(m_CacheArr[0].transform);
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
            m_Anim.SetTrigger(m_AttackTrigger);
            particleSystem.Play();
            SoundManager.Instance.PlaySound(punchSound);
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

    private void MoveToPosition(Transform target)
    {
        m_ReachedDestination = false;
        m_CurrentTarget = target;
    }

    public void MoveToPosition(Vector2 destination)
    {
        m_ReachedDestination = false;
        m_Destination = destination;
        m_CurrentTarget = null;
    }
}
