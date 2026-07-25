using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public abstract class Item : MonoBehaviour
{
    protected abstract void OnInteract(Player player);

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
            OnInteract(player);
    }

    public void Throw(Vector2 direction, float force)
    {
        _rb.AddForce(direction * force, ForceMode2D.Impulse);
    }
}
