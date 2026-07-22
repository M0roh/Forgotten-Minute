#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Layouts")]
    [SerializeField] private GameObject MainButtonsLayout;
    [SerializeField] private GameObject CreditsLayout;

    public void NewGameButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void SettingsButton() { }
    
    public void CreditsButton()
    {
        MainButtonsLayout.SetActive(false);
        CreditsLayout.SetActive(true);
    }

    public void CreditsBackButton()
    {
        MainButtonsLayout.SetActive(true);
        CreditsLayout.SetActive(false);
    }

    public void ExitButton()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
