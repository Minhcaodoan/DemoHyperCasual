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

    private void Start()
    {
        ShowMainMenu();
        if (Charactor != null)
        {
            Charactor.GetComponent<PlayerController>().enabled = false;
        }
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

        if (Charactor != null)
        {
            Charactor.GetComponent<PlayerController>().enabled = true;
        }
    }

    public void ShowGameOver()
    {
        mainMenuPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(true);

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + currentScoreText.text;
        }
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
        
    }

    public void OnClickReplayButton()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}