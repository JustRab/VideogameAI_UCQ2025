using UnityEngine;
using TMPro;

public class EnemyStatsUI : MonoBehaviour
{
    public GameObject statsPanel;
    public TextMeshProUGUI statsText;
    private bool visible = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            visible = !visible;
            statsPanel.SetActive(visible);
        }
    }

    public void ShowStats(EnemyStats stats)
    {
        float diff = DifficultyManager.Instance.EvaluateDifficulty(stats);
        float balance = DifficultyManager.Instance.EvaluateBalance(stats);
        float total = DifficultyManager.Instance.EvaluateTotalScore(stats);

        statsText.text =
            $"HP: {stats.HP:F2}\n" +
            $"Attack: {stats.AttackPower:F2}\n" +
            $"Rate: {stats.AttackRate:F2}\n" +
            $"Range: {stats.AttackRange:F2}\n" +
            $"Speed: {stats.MovementSpeed:F2}\n" +
            $"Fire: {(stats.fire ? "Yes" : "No")}, Poison: {(stats.poison ? "Yes" : "No")}, Stun: {(stats.stun ? "Yes" : "No")}\n" +
            $"Pattern: {stats.movementPatternWeight:F2}\n" +
            $"Difficulty: {diff:F2}\n" +
            $"Balance: {balance:F2}\n" +
            $"Total Score: {total:F2}";
    }
}
