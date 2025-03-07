using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI bestScoreText;
    [SerializeField] TextMeshProUGUI overallScoreText;
    [SerializeField] GameObject gameOver;
    [SerializeField] Parallax parallax;
    [SerializeField] AudioClip audioClip;
    [SerializeField] GameObject characterSelectionPanel;

    [Header("PlayerSelection")]
    private Player playerInstance;
    public static GameManager Instance { get; private set; }

    private AudioSource audioSource;
    private int score;
    private int bestScore;
    private int overallScore;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.playOnAwake = false;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Pause();

        // Load the best score from PlayerPrefs
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        overallScore = PlayerPrefs.GetInt("OverallScore", 0); // Load overall score from PlayerPrefs
        UpdateBestScoreText(); // Update the UI to show the best score
        UpdateOverallScoreText(); // Update the UI to show the overall score to show the best score
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
        player.enabled = true;

        playerInstance.transform.position = new Vector3(-3.5f, 0f, 0f);

        Pipes[] pipes = FindObjectsOfType<Pipes>();
        foreach (var pipe in pipes)
        {
            Destroy(pipe.gameObject);
        }

        parallax?.ResetMaterial();

        PlayAudio();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        player.enabled = false;
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
        
        StopAudio();
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
        bestScoreText.text = "Best Score: " + bestScore.ToString(); // Update the best score UI
    }

    private void UpdateOverallScoreText()
    {
        overallScoreText.text = "Overall Score: " + overallScore.ToString(); // Update the overall score UI
    }

    private void PlayAudio()
    {
        if (audioSource != null && audioClip != null)
        {
            audioSource.Play();
        }
    }

    private void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
