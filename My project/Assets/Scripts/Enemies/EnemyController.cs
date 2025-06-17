using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyStats stats;
    public Transform player;

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
        GetComponent<Renderer>().material.color = DifficultyManager.Instance.GetColorByDifficulty(difficulty);
    }

    void HandleMovement()
    {
        if (stats.MovementSpeed <= 0f) return; // torreta

        Vector3 dir = Vector3.zero;
        if (stats.movementPatternWeight >= 0.9f)
            dir = (player.position - transform.position).normalized; // persigue
        else if (stats.movementPatternWeight >= 0.4f)
            dir = -(player.position - transform.position).normalized; // huye
        else
            return; // estático

        Vector3 target = transform.position + dir * stats.MovementSpeed * 5f * Time.deltaTime;
        rb.MovePosition(target);
    }

    void HandleAttack()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= stats.AttackRange * 10f)
            {
                var ph = player.GetComponent<PlayerHealth>();
                if (ph != null)
                    ph.TakeDamage(stats.AttackPower * 10f);  // escala daño a vida real
                attackCooldown = stats.AttackRate * 2f;
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
