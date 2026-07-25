using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public abstract class Item : MonoBehaviour
{
    private float _spawnTime;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spawnTime = Time.time;
    }
    
    protected abstract void OnInteract(Player player);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.time - _spawnTime < 0.5f)
            return;

        if (collision.TryGetComponent(out Player player))
            OnInteract(player);
    }

    public void Throw(Vector2 direction, float force)
    {
        _rb.AddForce(direction * force, ForceMode2D.Impulse);
    }
}
