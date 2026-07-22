using UnityEngine;

public class GameInput : MonoBehaviour
{
    private static GameInput _instance;
    public static GameInput Instance => _instance;

    private InputActions _actions;
    public InputActions Actions => _actions;

    private void Awake()
    {
        if (_instance != null)
            Destroy(this);
        _instance = this;

        _actions = new();
    }

    private void OnEnable()
    {
        _actions.Player.Enable();
    }

    private void OnDisable()
    {
        _actions.Player.Disable();
    }

    public Vector2 GetMoveVector() => _actions.Player.Move.ReadValue<Vector2>();
}
