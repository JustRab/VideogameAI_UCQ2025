using System.Collections.Generic;
using UnityEngine;

public enum EnemyGenerationMethod
{
    Greedy,
    SimulatedAnnealing,
    Genetic
}

public class EnemyGenerator : MonoBehaviour
{
    // --------------------- AJUSTES GENERALES -----------------------
    [Header("Algoritmo de generación")]
    public EnemyGenerationMethod generationMethod = EnemyGenerationMethod.Greedy;

    [Header("Prefab y Spawn")]
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    // --------------------- GREEDY ----------------------------------
    [Header("Greedy Settings")]
    public int maxIterations = 30;

    // --------------------- SIMULATED ANNEALING ---------------------
    [Header("Simulated Annealing Settings")]
    public int saMaxIterations = 200;
    [Range(0f, 1f)] public float saInitialTemperature = 1f;
    [Range(0f, 1f)] public float saCoolingRate = 0.95f;

    // --------------------- GENETIC ---------------------------------
    [Header("Genetic Algorithm Settings")]
    public GeneticAlgorithmManager gaConfig = new GeneticAlgorithmManager();
    public int gaEnemiesReturned = 3;

    // ===============================================================
    // API PÚBLICA
    // ===============================================================
    /// <summary>
    /// Genera 'count' enemigos usando el algoritmo actualmente seleccionado
    /// y devuelve una lista con los GameObjects instanciados.
    /// </summary>
    public List<GameObject> GenerateEnemies(int count)
    {
        var list = new List<GameObject>();

        switch (generationMethod)
        {
            case EnemyGenerationMethod.Greedy:
                for (int i = 0; i < count; i++)
                    list.Add(SpawnEnemyGreedy(i));
                break;

            case EnemyGenerationMethod.SimulatedAnnealing:
                list.AddRange(GenerateWithSimulatedAnnealing(count));
                break;

            case EnemyGenerationMethod.Genetic:
                list.AddRange(GenerateWithGeneticAlgorithm());
                break;
        }
        return list;
    }

    // ===============================================================
    // GREEDY  (antes se llamaba SpawnEnemy)
    // ===============================================================
    GameObject SpawnEnemyGreedy(int indexOffset)
    {
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
        Debug.Log($"[{generationMethod}] Enemy spawned Score={bestScore:F2}");
        return SpawnFromStats(bestStats, indexOffset);
    }

    // ===============================================================
    // SIMULATED ANNEALING
    // ===============================================================
    List<GameObject> GenerateWithSimulatedAnnealing(int count)
    {
        var list = new List<GameObject>();

        for (int e = 0; e < count; e++)
        {
            EnemyStats current = EnemyStats.GenerateRandom();
            float currentScore = DifficultyManager.Instance.EvaluateTotalScore(current);
            float T = saInitialTemperature;

            for (int i = 0; i < saMaxIterations; i++)
            {
                EnemyStats neighbor = Perturb(current);
                float neighScore = DifficultyManager.Instance.EvaluateTotalScore(neighbor);
                float delta = neighScore - currentScore;

                if (delta > 0 || Random.value < Mathf.Exp(delta / T))
                {
                    current = neighbor;
                    currentScore = neighScore;
                }
                T *= saCoolingRate;
                if (T < 0.0001f) break;
            }
            list.Add(SpawnFromStats(current, e));
            Debug.Log($"[{generationMethod}] Enemy spawned Score={currentScore:F2}");
        }
        return list;
    }

    EnemyStats Perturb(EnemyStats o)
    {
        EnemyStats n = new EnemyStats
        {
            HP = Clamp01(o.HP + Random.Range(-0.2f, 0.2f)),
            AttackPower = Clamp01(o.AttackPower + Random.Range(-0.2f, 0.2f)),
            AttackRate = Mathf.Clamp(o.AttackRate + Random.Range(-0.1f, 0.1f), 0.1f, 1f),
            AttackRange = Clamp01(o.AttackRange + Random.Range(-0.2f, 0.2f)),
            MovementSpeed = Clamp01(o.MovementSpeed + Random.Range(-0.2f, 0.2f)),

            fire = o.fire ^ (Random.value < 0.05f),
            poison = o.poison ^ (Random.value < 0.05f),
            stun = o.stun ^ (Random.value < 0.05f),

            movementPatternWeight = Clamp01(o.movementPatternWeight + Random.Range(-0.2f, 0.2f))
        };
        return n;

        float Clamp01(float v) => Mathf.Clamp01(v);
    }

    // ===============================================================
    // GENETIC ALGORITHM
    // ===============================================================
    List<GameObject> GenerateWithGeneticAlgorithm()
    {
        var bestStats = gaConfig.Run(
            DifficultyManager.Instance.difficultyWeight,
            DifficultyManager.Instance.balanceWeight,
            gaEnemiesReturned);

        var list = new List<GameObject>();
        for (int i = 0; i < bestStats.Count; i++)
            list.Add(SpawnFromStats(bestStats[i], i));
        Debug.Log($"[{generationMethod}] Enemy spawned Stats={bestStats:F2}");
        return list;
    }

    // ===============================================================
    // HELPERS
    // ===============================================================
    GameObject SpawnFromStats(EnemyStats stats, int offset)
    {
        Vector3 pos = spawnPoint.position + new Vector3(offset * 2f, 0f, 0f);
        GameObject go = Instantiate(enemyPrefab, pos, Quaternion.identity);
        go.GetComponent<EnemyController>().Initialize(stats);

        // UI
        FindObjectOfType<EnemyStatsUI>().ShowStats(stats);
        return go;
    }
}
