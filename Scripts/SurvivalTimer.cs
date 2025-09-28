using UnityEngine;
using TMPro;          // TextMeshPro namespace
using UnityEngine.SceneManagement;

public class SurvivalTimerTMP : MonoBehaviour
{
    [Header("Timer Settings")]
    public float timeLimit = 60f;        // Survival time in seconds
    private float currentTime;

    [Header("UI")]
    public TextMeshProUGUI timerText;    // Assign TMP text element

    [Header("Player Reference")]
    public GameObject player;            // Assign the player GameObject

    private bool timerRunning = true;

    void Start()
    {
        currentTime = timeLimit;
        UpdateTimerText();
    }

    void Update()
    {
        if (!timerRunning) return;

        if (currentTime > 0f)
        {
            currentTime -= Time.deltaTime;
            if (currentTime < 0f) currentTime = 0f;

            UpdateTimerText();

            if (currentTime <= 0f)
            {
                timerRunning = false;
                CheckPlayerSurvival();
            }
        }
    }

    void UpdateTimerText()
    {
        if (timerText != null)
            timerText.text = "Time: " + Mathf.Ceil(currentTime).ToString();
    }

    void CheckPlayerSurvival()
    {
        if (player != null && player.activeInHierarchy)
        {
            Debug.Log("Time's up! Player survived ¡÷ YOU WIN!");
            // Example: Load win scene
            // SceneManager.LoadScene("WinScene");
        }
        else
        {
            Debug.Log("Player died before time ended ¡÷ GAME OVER!");
            // Example: Load lose scene
            // SceneManager.LoadScene("LoseScene");
        }
    }
}
