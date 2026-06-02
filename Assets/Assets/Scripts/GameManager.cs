using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

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

        UpdateUI();
    }

    private void Update()
    {
        if (isGameOver) return;

        survivalTime += Time.deltaTime;
        score = Mathf.FloorToInt(survivalTime) * scorePerSecond;

        UpdateUI();
    }

    public void TakePlayerDamage(int damage)
    {
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