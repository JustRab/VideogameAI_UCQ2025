using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyStats stats;
    public Transform player;
    public GeneralEnemyConfig config;

    private float attackCooldown;
    public float maxHP = 100f;
    private float currentHP;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleMovement();
        HandleAttack();
    }

    public void Initialize(EnemyStats newStats)
    {
        stats = newStats;
        currentHP = stats.HP * maxHP;

        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        float difficulty = DifficultyManager.Instance.EvaluateDifficulty(stats);
    }

    void HandleMovement()
    {
        if (stats.MovementSpeed <= 0f) return; // torreta

        Vector3 dir = Vector3.zero;
        if (stats.movementPatternWeight >= config.chaseThreshold)
            dir = (player.position - transform.position).normalized;
        else if (stats.movementPatternWeight >= config.fleeThreshold)
            dir = -(player.position - transform.position).normalized;
        else
            return;

        Vector3 target = transform.position + dir * stats.MovementSpeed * config.movementMultiplier * Time.deltaTime;
        rb.MovePosition(target);
    }

    void HandleAttack()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= stats.AttackRange * config.attackRangeMultiplier)
            {
                var ph = player.GetComponent<PlayerHealth>();
                if (ph != null)
                    ph.TakeDamage(stats.AttackPower * config.attackPowerMultiplier);

                attackCooldown = stats.AttackRate * config.attackCooldownMultiplier;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP <= 0f)
            Destroy(gameObject);
    }
}
