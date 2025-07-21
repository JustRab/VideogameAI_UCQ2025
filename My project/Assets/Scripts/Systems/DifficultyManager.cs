using UnityEngine;

public enum DifficultyFunctionType
{
    Version2,
    Version3,
    Custom1,
    Custom2
}

public class DifficultyManager : MonoBehaviour
{
    [Header("Configuración de Dificultad")]
    public DifficultyFunctionType selectedFunction;
    [Range(0f, 0.9f)] public float difficultyWeight = 0.5f;
    [Range(0f, 0.9f)] public float balanceWeight = 0.5f;

    private const float RED_THRESHOLD = 0.66f;
    private const float YELLOW_THRESHOLD = 0.33f;

    public static DifficultyManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public float EvaluateDifficulty(EnemyStats stats)
    {
        float result = 0f;
        switch (selectedFunction)
        {
            case DifficultyFunctionType.Version2:
                result = stats.HP + (stats.AttackPower / stats.AttackRate) + stats.AttackRange + stats.MovementSpeed;
                break;
            case DifficultyFunctionType.Version3:
                result = stats.HP + stats.AttackPower / stats.AttackRate + stats.AttackRange + stats.MovementSpeed + stats.UniqueEffectScore;
                break;
            case DifficultyFunctionType.Custom1:
                result = (stats.HP * stats.MovementSpeed) + Mathf.Sqrt(stats.AttackPower) + stats.AttackRange;
                break;
            case DifficultyFunctionType.Custom2:
                result = Mathf.Max(stats.HP, stats.AttackPower) + (1f / stats.AttackRate) + stats.UniqueEffectScore;
                break;
        }

        return Mathf.Clamp01(result / 5f); // normalización tentativa
    }

    public float EvaluateBalance(EnemyStats stats)
    {
        float total = stats.HP - 0.5f +
                      stats.AttackPower - 0.5f +
                      stats.AttackRate - 0.5f +
                      stats.AttackRange - 0.5f +
                      stats.MovementSpeed - 0.5f;

        return 1f - Mathf.Abs(Mathf.Clamp(total, -1f, 1f));
    }

    public float EvaluateTotalScore(EnemyStats stats)
    {
        float diff = EvaluateDifficulty(stats);
        float balance = EvaluateBalance(stats);

        float totalWeight = difficultyWeight + balanceWeight;
        float normalizedDiffWeight = difficultyWeight / totalWeight;
        float normalizedBalWeight = balanceWeight / totalWeight;

        return diff * normalizedDiffWeight + balance * normalizedBalWeight;
    }

}