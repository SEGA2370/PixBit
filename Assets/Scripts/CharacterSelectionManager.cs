using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject[] characters;
    [SerializeField] private Image panel;

    private int selectedIndex = 0;

    public void SelectCharacter(int index)
    {
        selectedIndex = index;
        UpdateCharacterSelection();
        StartGame();
    }

    private void UpdateCharacterSelection()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == selectedIndex);
        }
    }

    private void StartGame()
    {
        GameManager.Instance?.SetPlayerCharacter(characters[selectedIndex]);
        panel.gameObject.SetActive(false);
        Time.timeScale = 1;
        GameManager.Instance?.Play();
    }
}
