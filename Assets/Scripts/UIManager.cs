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

    [Header("Player Reference")]
    [SerializeField] private Transform Charactor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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

    public void ShowGameOver()
    {
        mainMenuPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        finalScoreText.text = "Score: "+currentScoreText.text+"m";
    }

    public void UpdateScoreUI()
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = Charactor.position.z.ToString("N0");
        }
    }

    public void OnClickPlayButton()
    {
        ShowGameplay();
        GameDemoManager.Instance.StartGame();
        AudioManager.Instance?.PlaySFX("ClickSound");
    }

    public void OnClickReplayButton()
    {
        GameDemoManager.Instance.RestartGame();
        AudioManager.Instance?.PlaySFX("ClickSound");
    }
}