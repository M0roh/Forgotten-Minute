using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Layouts")]
    [SerializeField] private GameObject PauseLayout;

    private void Start()
    {
        ContinueButton();
    }

    private void OnEnable()
    {
        GameInput.Instance.Actions.Menu.Pause.performed += Pause_performed;
    }

    private void OnDisable()
    {
        GameInput.Instance.Actions.Menu.Pause.performed -= Pause_performed;   
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        if (PauseLayout.activeSelf)
            ContinueButton();
        else
            Pause();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        PauseLayout.SetActive(true);
        GameInput.Instance.Actions.Player.Disable();
    }

    public void ContinueButton()
    {
        Time.timeScale = 1;
        PauseLayout.SetActive(false);
        GameInput.Instance.Actions.Player.Enable();
    }

    public void SettingsButton() { }

    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
