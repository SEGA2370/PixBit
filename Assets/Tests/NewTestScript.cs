using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    private GameObject gameManagerObj;
    private GameManager gameManager;
    private GameObject playerObj;
    private Player player;

    [SetUp]
    public void Setup()
    {
        // Clear any existing instance using the new method
        GameManager.ResetInstance();

        // Create fresh objects
        gameManagerObj = new GameObject("GameManager");
        playerObj = new GameObject("Player");

        // Create minimal required components
        var scoreText = new GameObject("ScoreText").AddComponent<TextMeshProUGUI>();
        var gameOverObj = new GameObject("GameOver");
        var characterSelectionPanel = new GameObject("CharacterSelectionPanel");

        // Setup GameManager
        gameManager = gameManagerObj.AddComponent<GameManager>();
        gameManager.scoreText = scoreText;
        gameManager.gameOver = gameOverObj;
        gameManager.characterSelectionPanel = characterSelectionPanel;
        gameManager.audioClip = AudioClip.Create("TestClip", 44100, 1, 44100, false);

        // Setup Player
        player = playerObj.AddComponent<Player>();
        playerObj.AddComponent<BoxCollider2D>();
        var rb = playerObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.isKinematic = true;

        // Initialize
        gameManager.Awake();
        gameManager.player = player;
    }

    [TearDown]
    public void Teardown()
    {
        // Use the reset method instead of direct access
        GameManager.ResetInstance();

        // Clean up any remaining objects
        var objects = Object.FindObjectsOfType<GameObject>();
        foreach (var obj in objects)
        {
            if (obj != null && obj.scene.name == null) // Only destroy DontDestroyOnLoad objects
            {
                Object.DestroyImmediate(obj);
            }
        }
    }

    [UnityTest]
    public IEnumerator BackgroundMusicPlaysWhenGameStarts()
    {
        // Act
        gameManager.Play();

        // Wait for audio to initialize
        yield return new WaitForSeconds(0.1f);

        // Assert
        var audioSource = gameManager.GetAudioSource();
        Assert.IsNotNull(audioSource, "AudioSource is null");
        Assert.IsTrue(audioSource.isPlaying, $"Audio is not playing. State: {audioSource.isPlaying}, Clip: {audioSource.clip?.name}");
    }

    [UnityTest]
    public IEnumerator BackgroundMusicStopsWhenGameOver()
    {
        // Start game
        gameManager.Play();
        yield return new WaitForSeconds(0.1f);

        // Verify playing
        Assert.IsTrue(gameManager.GetAudioSource().isPlaying);

        // Trigger GameOver
        gameManager.GameOver();
        yield return new WaitForSeconds(0.2f); // Slightly longer wait for coroutine

        // Verify stopped
        Assert.IsFalse(gameManager.GetAudioSource().isPlaying);
    }

    [UnityTest]
    public IEnumerator PlayerCollisionWithScoring_IncreasesScore()
    {
        // Arrange
        gameManager.Play();
        int initialScore = gameManager.score;

        // Create scoring trigger collider
        var scoringObj = new GameObject("ScoringTrigger");
        scoringObj.tag = "Scoring";
        var scoringCollider = scoringObj.AddComponent<BoxCollider2D>();
        scoringCollider.isTrigger = true;
        scoringObj.transform.position = playerObj.transform.position;

        // Disable player movement during test
        player.enabled = false;

        // Act - Wait for physics update
        yield return new WaitForFixedUpdate();

        // Assert
        Assert.AreEqual(initialScore + 1, gameManager.score,
            "Score should increase by 1 when colliding with scoring object");

        // Cleanup
        Object.DestroyImmediate(scoringObj);
    }

    [UnityTest]
    public IEnumerator Player_ReverseGravity_MakesPlayerFloatUp()
    {
        // Arrange
        gameManager.Play();
        player.enabled = true;

        // Store initial position
        Vector3 initialPosition = player.transform.position;
        float initialY = initialPosition.y;

        // Act - Wait for a few frames to let gravity take effect
        yield return new WaitForSeconds(0.3f);

        // Assert
        // Ensure the player floats upwards (y should increase)
        float newY = player.transform.position.y;
        Assert.Greater(newY, initialY, "Player should be floating up, meaning y should increase");

        // Verify the direction's y value is positive (indicating upward movement)
        Assert.Greater(player.direction.y, 0, "Direction.y should be positive, indicating upward movement");
    }

    [UnityTest]
    public IEnumerator Spawner_InstantiatesPrefab()
    {
        // Arrange
        var spawnerObj = new GameObject("Spawner");
        var spawner = spawnerObj.AddComponent<Spawner>();
        var testPrefab = new GameObject("TestPrefab");
        spawner.prefab = testPrefab;

        // Act - Force spawn
        spawner.Spawn();
        yield return null; // Wait one frame

        // Assert - Check if any instances exist
        var instances = GameObject.FindObjectsOfType<GameObject>();
        bool foundInstance = false;
        foreach (var obj in instances)
        {
            if (obj != testPrefab && obj.name.Contains("TestPrefab"))
                foundInstance = true;
        }
        Assert.IsTrue(foundInstance, "Spawner should instantiate the prefab");

        // Cleanup
        Object.DestroyImmediate(spawnerObj);
        Object.DestroyImmediate(testPrefab);
    }
}