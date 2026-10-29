using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        ShowMainMenu();
    }

    
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void ShowGameplay()
    {
        mainMenuPanel.SetActive(false);
        gameplayPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver(int finalScore)
    {
        mainMenuPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(true);

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + finalScore.ToString();
        }
    }

    public void UpdateScoreUI(int score)
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = score.ToString();
        }
    }

    public void OnClickPlayButton()
    {
        ShowGameplay();
    }

    public void OnClickReplayButton()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}