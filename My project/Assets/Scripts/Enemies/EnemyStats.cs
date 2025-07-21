using UnityEngine;

[System.Serializable]
public class EnemyStats
{

    [Range(0f, 1f)] public float HP;
    [Range(0f, 1f)] public float AttackPower;
    [Range(0f, 1f)] public float AttackRate; // The lower the value, the faster the attack rate
    [Range(0f, 1f)] public float AttackRange;
    [Range(0f, 1f)] public float MovementSpeed;
    public EnemyType enemyType;

    // VFX
    public bool fire;
    public bool poison;
    public bool stun;

    // To balance: 0 = no movement, 0.5 = flees, 1 = charges
    [Range(0f, 1f)] public float movementPatternWeight;

    public float UniqueEffectScore =>
        (fire ? 0.3f : 0f) + (poison ? 0.3f : 0f) + (stun ? 0.4f : 0f);

    public static EnemyStats GenerateRandom()
    {
        EnemyStats stats = new EnemyStats();

        stats.HP = Random.Range(0f, 1f);
        stats.AttackPower = Random.Range(0f, 1f);
        stats.AttackRate = Random.Range(0.1f, 1f); // evita división por cero
        stats.AttackRange = Random.Range(0f, 1f);
        stats.MovementSpeed = Random.Range(0f, 1f);

        // Efectos especiales aleatorios (30% probabilidad cada uno)
        stats.fire = Random.value < 0.3f;
        stats.poison = Random.value < 0.3f;
        stats.stun = Random.value < 0.3f;

        // Movimiento aleatorio
        stats.movementPatternWeight = Random.Range(0f, 1f);

        return stats;
    }
}

