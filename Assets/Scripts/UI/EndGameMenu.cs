using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject EndGameLayout;
    [SerializeField] private TMP_Text _gameResultText;

    [Header("Text")]
    [SerializeField] private string _victoryText;
    [SerializeField] private string _gameOverText;

    private void Start()
    {
        EndGameLayout.SetActive(false);
    }

    private void OnEnable()
    {
        RoomController.Instance.OnBossesDeath += VictoryGame;
        Player.Instance.OnDeath += GameOver;
    }

    private void OnDisable()
    {
        RoomController.Instance.OnBossesDeath -= VictoryGame;
        Player.Instance.OnDeath -= GameOver;
    }

    private void VictoryGame()
    {
        Pause();
        _gameResultText.text = _victoryText;
    }

    private void GameOver()
    {
        Pause();
        _gameResultText.text = _gameOverText;
    }

    public void Pause()
    {
        Time.timeScale = 0;
        EndGameLayout.SetActive(true);
        GameInput.Instance.Actions.Disable();
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
