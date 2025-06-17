using UnityEngine;

// Si ve este profesor, todo esto hecho fue con el poder de cristo y "Apex of The World" de "Fire Emblem: Three Houses" de fondo. 
public class EnemyFactory : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;

    public EnemyController CreateRandomEnemy()
    {
        GameObject enemyGO = Instantiate(enemyPrefab, GetRandomSpawnPoint(), Quaternion.identity);
        var controller = enemyGO.GetComponent<EnemyController>();

        controller.player = player;
        controller.stats = GenerateRandomStats();
        FindObjectOfType<EnemyStatsUI>().ShowStats(controller.stats);

        return controller;
    }

    EnemyStats GenerateRandomStats()
    {
        return new EnemyStats
        {
            HP = Random.value,
            AttackPower = Random.value,
            AttackRate = Random.Range(0.1f, 1f),
            AttackRange = Random.value,
            MovementSpeed = Random.value,
            fire = Random.value < 0.2f,
            poison = Random.value < 0.2f,
            stun = Random.value < 0.1f,
            movementPatternWeight = Random.value,

        };
    }

    Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
    }
}
