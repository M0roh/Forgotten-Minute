using UnityEngine;
using UnityEngine.Events;

public class ColliderTrigger : MonoBehaviour
{
    [SerializeField] protected UnityEvent _onEnter;
    [SerializeField] protected UnityEvent _onStay;
    [SerializeField] protected UnityEvent _onExit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        _onEnter?.Invoke();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        _onStay?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _onExit?.Invoke();
    }
}