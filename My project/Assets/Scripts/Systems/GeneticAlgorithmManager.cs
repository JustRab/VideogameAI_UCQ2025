using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneticAlgorithmManager
{
    // ---------- Parámetros públicos --------------
    public int populationSize = 20;
    public int generations = 25;
    public int eliteCount = 2;     // se copian sin mutar
    [Range(0f, 1f)] public float crossoverRate = 0.8f;
    [Range(0f, 1f)] public float mutationRate = 0.1f;

    // ---------- Pipeline público -----------------
    public List<EnemyStats> Run(float difficultyWeight, float balanceWeight, int needed)
    {
        // 1. Initialization
        List<EnemyStats> population = Initialization();

        for (int g = 0; g < generations; g++)
        {
            // 2. Fitness assignment
            List<float> fitness = FitnessAssignment(population, difficultyWeight, balanceWeight);

            // 3. Selection
            List<EnemyStats> matingPool = Selection(population, fitness);

            // 4. Crossover + Mutation
            population = CrossoverMutation(matingPool);
        }

        // Re‑evalúa última generación y ordena por fitness desc.
        List<float> finalFit = FitnessAssignment(population, difficultyWeight, balanceWeight);
        population.Sort((a, b) =>
            FitnessOf(b, difficultyWeight, balanceWeight)
            .CompareTo(
            FitnessOf(a, difficultyWeight, balanceWeight)));

        // Devuelve los 'needed' mejores (o la población si es menor)
        return population.GetRange(0, Mathf.Min(needed, population.Count));
    }

    // ------------------------------------------------------
    // 1) Initialization
    List<EnemyStats> Initialization()
    {
        var pop = new List<EnemyStats>(populationSize);
        for (int i = 0; i < populationSize; i++)
            pop.Add(EnemyStats.GenerateRandom());
        return pop;
    }

    // 2) Fitness assignment
    List<float> FitnessAssignment(List<EnemyStats> pop,
                                  float dW, float bW)
    {
        var list = new List<float>(pop.Count);
        foreach (var s in pop)
            list.Add(FitnessOf(s, dW, bW));
        return list;
    }
    float FitnessOf(EnemyStats s, float dW, float bW)
    {
        DifficultyManager.Instance.difficultyWeight = dW;
        DifficultyManager.Instance.balanceWeight = bW;
        return DifficultyManager.Instance.EvaluateTotalScore(s);
    }

    // 3) Selection – torneo de tamaño 3
    List<EnemyStats> Selection(List<EnemyStats> pop, List<float> fit)
    {
        var pool = new List<EnemyStats>();

        // Elitismo
        (pop, fit) = SortDescending(pop, fit);
        for (int i = 0; i < eliteCount; i++)
            pool.Add(pop[i]);

        // Torneos
        while (pool.Count < populationSize)
        {
            EnemyStats a = pop[Random.Range(0, pop.Count)];
            EnemyStats b = pop[Random.Range(0, pop.Count)];
            EnemyStats c = pop[Random.Range(0, pop.Count)];

            EnemyStats winner = FitnessOf(a, 0, 0) > FitnessOf(b, 0, 0)
                             ? (FitnessOf(a, 0, 0) > FitnessOf(c, 0, 0) ? a : c)
                             : (FitnessOf(b, 0, 0) > FitnessOf(c, 0, 0) ? b : c);
            pool.Add(winner);
        }
        return pool;
    }

    // 4) Crossover + Mutation
    List<EnemyStats> CrossoverMutation(List<EnemyStats> pool)
    {
        var next = new List<EnemyStats>();

        // Copia élite directo
        for (int i = 0; i < eliteCount; i++)
            next.Add(Clone(pool[i]));

        // Resto por parejas
        while (next.Count < populationSize)
        {
            EnemyStats parent1 = pool[Random.Range(0, pool.Count)];
            EnemyStats parent2 = pool[Random.Range(0, pool.Count)];

            EnemyStats child = Random.value < crossoverRate
                             ? Crossover(parent1, parent2)
                             : Clone(parent1);

            Mutate(child);
            next.Add(child);
        }
        return next;
    }

    // ----------------- Utilidades ------------------
    EnemyStats Clone(EnemyStats o) => JsonUtility.FromJson<EnemyStats>(
                                      JsonUtility.ToJson(o));

    EnemyStats Crossover(EnemyStats a, EnemyStats b)
    {
        EnemyStats c = new EnemyStats();
        c.HP = Random.value < 0.5f ? a.HP : b.HP;
        c.AttackPower = Random.value < 0.5f ? a.AttackPower : b.AttackPower;
        c.AttackRate = Random.value < 0.5f ? a.AttackRate : b.AttackRate;
        c.AttackRange = Random.value < 0.5f ? a.AttackRange : b.AttackRange;
        c.MovementSpeed = Random.value < 0.5f ? a.MovementSpeed : b.MovementSpeed;

        c.fire = Random.value < 0.5f ? a.fire : b.fire;
        c.poison = Random.value < 0.5f ? a.poison : b.poison;
        c.stun = Random.value < 0.5f ? a.stun : b.stun;

        c.movementPatternWeight = Random.value < 0.5f ?
                                  a.movementPatternWeight :
                                  b.movementPatternWeight;
        return c;
    }

    void Mutate(EnemyStats s)
    {
        if (Random.value < mutationRate) s.HP = Random.value;
        if (Random.value < mutationRate) s.AttackPower = Random.value;
        if (Random.value < mutationRate) s.AttackRate = Random.Range(0.1f, 1f);
        if (Random.value < mutationRate) s.AttackRange = Random.value;
        if (Random.value < mutationRate) s.MovementSpeed = Random.value;

        if (Random.value < mutationRate) s.fire = !s.fire;
        if (Random.value < mutationRate) s.poison = !s.poison;
        if (Random.value < mutationRate) s.stun = !s.stun;

        if (Random.value < mutationRate)
            s.movementPatternWeight = Random.value;
    }

    // Ordenar listas en paralelo
    (List<EnemyStats>, List<float>) SortDescending(List<EnemyStats> pop, List<float> fit)
    {
        for (int i = 0; i < pop.Count - 1; i++)
            for (int j = i + 1; j < pop.Count; j++)
                if (fit[j] > fit[i])
                {
                    (pop[i], pop[j]) = (pop[j], pop[i]);
                    (fit[i], fit[j]) = (fit[j], fit[i]);
                }
        return (pop, fit);
    }
}
