using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class HordeManager : MonoBehaviour
{
    [Header("Refs")]
    public EnemyGenerator generator;          // Asigna tu EnemyGenerator
    public PlayerHealth playerHealth;         // Asigna tu PlayerHealth

    [Header("Parámetros de Horda")]
    public int enemiesPerRound = 3;
    public float timeBetweenRounds = 2f;

    [Header("Pesos Iniciales (≤0.9)")]
    [Range(0f, 0.9f)] public float difficultyWeight = 0.3f;
    [Range(0f, 0.9f)] public float balanceWeight = 0.7f;

    [Header("UI (TextMeshProUGUI)")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI weightsText;
    public TextMeshProUGUI resultText;

    private int currentRound = 0;
    public List<GameObject> activeEnemies = new List<GameObject>();
    private bool roundActive = false;

    void Start()
    {
        // Inicializar UI
        resultText.gameObject.SetActive(false);
        StartCoroutine(StartNextRound());
    }

    void Update()
    {
        if (!roundActive) return;

        // Limpiar referencias a enemigos destruidos
        activeEnemies.RemoveAll(e => e == null);

        if (activeEnemies.Count == 0)
        {
            // El jugador venció la ronda
            roundActive = false;
            resultText.text = "¡Ronda " + currentRound + " completada!";
            StartCoroutine(RoundEnd(true));
        }
    }

    IEnumerator StartNextRound()
    {
        currentRound++;
        roundText.text = "Ronda: " + currentRound;
        UpdateWeightsUI();

        yield return new WaitForSeconds(timeBetweenRounds);

        // Ajustar los pesos globales antes de generar
        DifficultyManager.Instance.difficultyWeight = difficultyWeight;
        DifficultyManager.Instance.balanceWeight = balanceWeight;

        // Generar la horda
        activeEnemies.Clear();
        for (int i = 0; i < enemiesPerRound; i++)
        {
            GameObject e = generator.SpawnEnemy();
            activeEnemies.Add(e);
        }

        resultText.gameObject.SetActive(false);
        roundActive = true;
    }

    IEnumerator RoundEnd(bool playerWon)
    {
        // Mostrar resultado
        resultText.gameObject.SetActive(true);
        resultText.color = playerWon ? Color.green : Color.red;
        resultText.text = playerWon ? "¡Ganaste Ronda!" : "¡Perdiste Ronda!";

        // Ajustar pesos
        if (playerWon)
            difficultyWeight = Mathf.Min(0.9f, difficultyWeight + 0.1f);
        else
            difficultyWeight = Mathf.Max(0.1f, difficultyWeight - 0.1f);
        balanceWeight = 1f - difficultyWeight;

        UpdateWeightsUI();

        yield return new WaitForSeconds(2f);
        StartCoroutine(StartNextRound());
    }

    void UpdateWeightsUI()
    {
        weightsText.text = 
            $"DifficultyWeight: {difficultyWeight:F2}\n" +
            $"BalanceWeight:   {balanceWeight:F2}";
    }

    // Llamado desde PlayerHealth
    public void OnPlayerDeath()
    {
        if (!roundActive) return;
        roundActive = false;
        StartCoroutine(RoundEnd(false));
    }
}
