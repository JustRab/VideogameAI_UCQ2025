using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 velocity;
    public float lifetime = 2f;
    public float damage = 10f;

    // Llamado por PlayerShooting
    public void Initialize(Vector2 direction, float speed)
    {
        velocity = new Vector3(direction.x, 0, direction.y) * speed;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Infligir daño al enemigo
            var ec = other.GetComponent<EnemyController>();
            if (ec != null)
                ec.TakeDamage(damage);

            Destroy(gameObject);
        }

        // Opcional: destruye al tocar paredes
        if (other.CompareTag("Wall"))
            Destroy(gameObject);
    }
}
