using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameDemoManager : MonoBehaviour
{
    public static GameDemoManager Instance { get; private set; }
    private Vector3 startPosition;

    [Header("References")]
    [SerializeField] private Transform charactor; // Kéo thả nhân vật vào đây
    [Header("DelayTime")]
    [SerializeField] private float startDelay = 2f;
    private void Awake()
    {
        Application.targetFrameRate = 60; // Giới hạn 60 khung hình/giây
        QualitySettings.vSyncCount = 0;   // Tắt VSync để kiểm soát FPS tốt hơn trên mobile
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        AudioManager.Instance?.PlayBGM("MenuSound");
        UIManager.Instance?.ShowMainMenu();
        if (charactor != null)
        {
            charactor.GetComponent<PlayerController>().enabled = false;
        }
    }
    private IEnumerator StartGameRoutine()
    {
        UIManager.Instance?.ShowGameplay();
        yield return new WaitForSeconds(startDelay);
        if (charactor != null)
        {
            charactor.GetComponent<PlayerController>().enabled = true;
        }
    }

    private void Update()
    {
        UIManager.Instance?.UpdateScoreUI();
    }

    public void StartGame()
    {
        AudioManager.Instance?.StartRandomBGM();
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator OverRoutine()
    {
        AudioManager.Instance?.StopRandomBGM();
        AudioManager.Instance?.PlaySFX("DeadSound");
        if (charactor != null)
        {
            charactor.GetComponent<PlayerController>().enabled = false;
        }
        yield return new WaitForSeconds(1f);
        AudioManager.Instance?.PlaySFX("DeadSystemSound");
        yield return new WaitForSeconds(2f);
        UIManager.Instance?.ShowGameOver();
        AudioManager.Instance?.PlayBGM("OverSound");
    }
    public void GameOver()
    {
        StartCoroutine(OverRoutine());
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
