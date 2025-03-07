using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector3 direction;
    [SerializeField] float gravity = 5f;
    [SerializeField] float strength = 5f;
    [SerializeField] AudioClip deathSound; // Assign the death sound clip in the Inspector

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            direction = Vector3.down * strength;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                direction = Vector3.down * strength;
            }
        }

        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;
    }
    private void OnEnable()
    {
        Vector3 position = transform.position;
        position.y = 0;
        transform.position = position;
        direction = Vector3.zero;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            PlaySound(deathSound);
            FindObjectOfType<GameManager>().GameOver();
        }
        else if (collision.CompareTag("Scoring"))
        {
            FindObjectOfType<GameManager>().IncreaseScore();
        }
    }
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
        else
        {
            Debug.LogWarning("AudioClip is not assigned.");
        }
    }
}
