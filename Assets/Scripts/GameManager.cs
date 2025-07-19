using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;


public class GameManager : MonoBehaviour
{
    public static event System.Action OnGameOver;

    public Player player;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI overallScoreText;
    public GameObject gameOver;
    [SerializeField] Parallax parallax;
    public AudioClip audioClip;
    public GameObject characterSelectionPanel;
    public AudioSource audioSource;
    public bool enableAudio = true;
    private Spawner spawner;


    [Header("PlayerSelection")]
    private Player playerInstance;
    public static GameManager Instance { get; private set; }


    public int score;
    public int bestScore;
    public int overallScore;

    public void Awake()
    {
        Application.targetFrameRate = 60;

        spawner = FindObjectOfType<Spawner>();

        // Handle singleton pattern
        if (Instance == null)
        {
            Instance = this;
            // Only use DontDestroyOnLoad in actual game
            if (!IsInTestMode())
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (gameOver != null)
        {
            gameOver.SetActive(false);
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        Pause();

        // Load the best score from PlayerPrefs
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        overallScore = PlayerPrefs.GetInt("OverallScore", 0); // Load overall score from PlayerPrefs
        UpdateBestScoreText(); // Update the UI to show the best score
        UpdateOverallScoreText(); // Update the UI to show the overall score to show the best score
    }

    private bool IsInTestMode()
    {
#if UNITY_INCLUDE_TESTS
        return true;
#else
        return false;
#endif
    }
    public static void ResetInstance()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }
    }

    // Public method to access the audioSource for testing
    public AudioSource GetAudioSource()
    {
        return audioSource;
    }

    public void SetPlayerCharacter(GameObject characterPrefab)
    {
        if (playerInstance != null)
        {
            Destroy(playerInstance.gameObject);
        }

        GameObject newPlayer = Instantiate(characterPrefab);
        newPlayer.transform.position = new Vector3(-3.5f, 0f, 0f);
        playerInstance = newPlayer.GetComponent<Player>();
    }

    public void Play()
    {
        score = 0;
        scoreText.text = score.ToString();

        gameOver.SetActive(false);
        characterSelectionPanel.SetActive(false);

        Time.timeScale = 1;
        if (player != null)
        {
            player.enabled = true;
        }

        if (playerInstance != null)
        {
            playerInstance.transform.position = new Vector3(-3.5f, 0f, 0f);
        }

        Pipes[] pipes = FindObjectsOfType<Pipes>();
        foreach (var pipe in pipes)
        {
            Destroy(pipe.gameObject);
        }

        parallax?.ResetMaterial();

        spawner?.StartSpawning();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        if (player != null) //  Prevents NullReferenceException
        {
            player.enabled = false;
        }
    }

    public void GameOver()
    {
        // Check if the current score is greater than the best score
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore); // Save the new best score
            PlayerPrefs.Save(); // Ensure the data is saved
            UpdateBestScoreText(); // Update the UI to show the new best score
        }

        overallScore += score; // Update overall score
        PlayerPrefs.SetInt("OverallScore", overallScore); // Save overall score
        PlayerPrefs.Save(); // Ensure the data is saved
        UpdateOverallScoreText(); // Update the UI to show the overall score

        StartCoroutine(DelayedGameOverUI());
        OnGameOver?.Invoke();
        StopAudio();

        spawner?.StopSpawning();
    }

    private IEnumerator DelayedGameOverUI()
    {
        yield return new WaitForSeconds(0.2f);
        Pause();
        gameOver.SetActive(true);
        characterSelectionPanel.SetActive(true);
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    private void UpdateBestScoreText()
    {
        if (bestScoreText != null)
        {
            bestScoreText.text = "Best Score: " + bestScore.ToString();
        }
        else
        {
            Debug.LogWarning("bestScoreText is not assigned in GameManager");
        } // Update the best score UI
    }

    private void UpdateOverallScoreText()
    {
        if (overallScoreText != null)
        {
            overallScoreText.text = "Overall Score: " + overallScore.ToString();
        }
        else
        {
            Debug.LogWarning("overallScoreText is not assigned in GameManager");
        }
    }

    private void PlayAudio()
    {
        if (!enableAudio) return;

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (audioClip == null)
        {
            Debug.LogWarning("No audio clip assigned to GameManager");
            return;
        }

        audioSource.enabled = true;
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.volume = 1f;

        Debug.Log($"Attempting to play audio. Enabled: {audioSource.enabled}, Clip: {audioClip.name}");
        audioSource.Play();

        audioSource.ignoreListenerPause = true;
        AudioListener.pause = false;
    }

    private void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
