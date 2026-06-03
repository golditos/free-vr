using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    [SerializeField] private bool gameStarted = false;
    [SerializeField] private EnemySpawner[] enemySpawners;

    [Header("Player")]
    [SerializeField] private int maxLives = 3;
    private int currentLives;

    [Header("Score")]
    [SerializeField] private int scorePerSecond = 1;
    private float survivalTime = 0f;
    private int score = 0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject startTextObject;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private bool isGameOver = false;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        currentLives = maxLives;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (startTextObject != null)
        {
            startTextObject.SetActive(true);
        }

        StopAllSpawners();
        UpdateUI();
    }

    private void Update()
    {
        if (!gameStarted) return;
        if (isGameOver) return;

        survivalTime += Time.deltaTime;
        score = Mathf.FloorToInt(survivalTime) * scorePerSecond;

        UpdateUI();
    }

    public void StartGame()
    {
        if (gameStarted) return;
        if (isGameOver) return;

        gameStarted = true;
        survivalTime = 0f;
        score = 0;

        if (startTextObject != null)
        {
            startTextObject.SetActive(false);
        }

        StartAllSpawners();

        Debug.Log("Game started");
        UpdateUI();
    }

    public void TakePlayerDamage(int damage)
    {
        if (!gameStarted) return;
        if (isGameOver) return;

        currentLives -= damage;
        currentLives = Mathf.Max(currentLives, 0);

        UpdateUI();

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;

        StopAllSpawners();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + score;
        }

        Time.timeScale = 0f;
    }

    private void StartAllSpawners()
    {
        foreach (EnemySpawner spawner in enemySpawners)
        {
            if (spawner != null)
            {
                spawner.StartSpawning();
            }
        }
    }

    private void StopAllSpawners()
    {
        foreach (EnemySpawner spawner in enemySpawners)
        {
            if (spawner != null)
            {
                spawner.StopSpawning();
            }
        }
    }

    private void UpdateUI()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + currentLives;
        }

        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}