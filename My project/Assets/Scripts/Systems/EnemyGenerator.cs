using UnityEngine;
using System.Collections.Generic;

public class EnemyGenerator : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public HordeManager hordeManager;

    [Header("Greedy Settings")]
    public int maxIterations = 30;
    public int enemiesPerWave = 3;

    public void GenerateWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            EnemyStats bestStats = null;
            float bestScore = -1f;

            for (int j = 0; j < maxIterations; j++)
            {
                EnemyStats candidate = EnemyStats.GenerateRandom(); // Esta función ya la hicimos
                float score = DifficultyManager.Instance.EvaluateTotalScore(candidate);

                if (score > bestScore)
                {
                    bestStats = candidate;
                    bestScore = score;
                }
            }

            GameObject enemyGO = Instantiate(enemyPrefab, spawnPoint.position + new Vector3(i * 2, 0, 0), Quaternion.identity);
            EnemyController controller = enemyGO.GetComponent<EnemyController>();
            controller.Initialize(bestStats);

            FindObjectOfType<EnemyStatsUI>().ShowStats(bestStats); // Mostrar el último generado
        }
    }

    public GameObject SpawnEnemy()
    {
        // GreedySearch: encuentra bestStats
        EnemyStats bestStats = null;
        float bestScore = -1f;
        for (int i = 0; i < maxIterations; i++)
        {
            EnemyStats candidate = EnemyStats.GenerateRandom();
            float score = DifficultyManager.Instance.EvaluateTotalScore(candidate);
            if (score > bestScore)
            {
                bestScore = score;
                bestStats = candidate;
            }
        }

        // Instancia y configura
        Vector3 pos = spawnPoint.position + new Vector3(hordeManager.activeEnemies.Count * 2, 0, 0);
        GameObject go = Instantiate(enemyPrefab, pos, Quaternion.identity);
        var ctrl = go.GetComponent<EnemyController>();
        ctrl.Initialize(bestStats);
        FindObjectOfType<EnemyStatsUI>().ShowStats(bestStats);

        return go;
    }

}
