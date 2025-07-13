using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HordeManager : MonoBehaviour
{
    // -------------------- Referencias -------------------------
    [Header("Refs")]
    public EnemyGenerator generator;      // arrastra tu EnemyGenerator
    public PlayerHealth   playerHealth;   // arrastra tu PlayerHealth

    // -------------------- Parámetros de horda -----------------
    [Header("Parámetros de Horda")]
    public int enemiesPerRound = 3;
    public float timeBetweenRounds = 2f;

    // -------------------- Pesos -------------------------------
    [Header("Pesos Iniciales (≤0.9)")]
    [Range(0f, 0.9f)] public float difficultyWeight = 0.3f;
    [Range(0f, 0.9f)] public float balanceWeight    = 0.7f;

    // -------------------- UI ----------------------------------
    [Header("UI")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI weightsText;
    public TextMeshProUGUI resultText;

    // -------------------- Estado interno ----------------------
    int currentRound = 0;
    bool roundActive = false;
    readonly List<GameObject> activeEnemies = new List<GameObject>();

    // ==========================================================
    void Start()
    {
        resultText.gameObject.SetActive(false);
        StartCoroutine(StartNextRound());
    }

    void Update()
    {
        if (!roundActive) return;

        // limpia referencias
        activeEnemies.RemoveAll(e => e == null);

        if (activeEnemies.Count == 0)
        {
            roundActive = false;
            StartCoroutine(RoundEnd(true));
        }
    }

    // ==========================================================
    IEnumerator StartNextRound()
    {
        currentRound++;
        roundText.text = $"Ronda: {currentRound}";
        UpdateWeightsUI();

        yield return new WaitForSeconds(timeBetweenRounds);

        DifficultyManager.Instance.difficultyWeight = difficultyWeight;
        DifficultyManager.Instance.balanceWeight    = balanceWeight;

        // --- Generar horda con el algoritmo seleccionado ---
        activeEnemies.Clear();
        activeEnemies.AddRange(generator.GenerateEnemies(enemiesPerRound));

        resultText.gameObject.SetActive(false);
        roundActive = true;
    }

    IEnumerator RoundEnd(bool playerWon)
    {
        resultText.gameObject.SetActive(true);
        resultText.color = playerWon ? Color.green : Color.red;
        resultText.text  = playerWon ? "¡Ganaste la ronda!" : "¡Perdiste la ronda!";

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
