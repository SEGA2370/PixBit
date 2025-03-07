// CharacterSelectionManager.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject[] characters;
    [SerializeField] private Image panel;
    [SerializeField] private AdManager adManager;

    private int selectedIndex = 0;

    private void Start()
    {
        if (adManager != null)
        {
            adManager.OnAdClosed += ResumeGameAfterAd;
        }
    }

    public void SelectCharacter(int index)
    {
        selectedIndex = index;
        UpdateCharacterSelection();

        if (adManager?.CanShowAd() == true)
        {
            PauseGame();
            adManager.ShowAd();
        }
        else
        {
            StartGame();
        }
    }

    private void UpdateCharacterSelection()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == selectedIndex);
        }
    }

    private void PauseGame()
    {
        GameManager.Instance?.Pause();
    }

    private void ResumeGameAfterAd()
    {
        StartCoroutine(ResumeGameWithDelay());
    }

    private IEnumerator ResumeGameWithDelay()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        GameManager.Instance?.Play();
    }

    private void StartGame()
    {
        GameManager.Instance?.SetPlayerCharacter(characters[selectedIndex]);
        panel.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    private void OnDestroy()
    {
        if (adManager != null)
        {
            adManager.OnAdClosed -= ResumeGameAfterAd;
        }
    }
}
