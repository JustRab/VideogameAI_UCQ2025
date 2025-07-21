using UnityEngine;

[CreateAssetMenu(fileName = "GeneralEnemyConfig", menuName = "Configs/GeneralEnemyConfig")]
public class GeneralEnemyConfig : ScriptableObject
{
    [Header("Movement")]
    public float chaseThreshold = 0.9f;
    public float fleeThreshold = 0.4f;
    public float movementMultiplier = 5f;

    [Header("Attack")]
    public float attackRangeMultiplier = 10f;
    public float attackCooldownMultiplier = 2f;
    public float attackPowerMultiplier = 10f;
}
