using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Refs")]
    public Transform firePoint;         // Punto de salida de la bala
    public GameObject bulletPrefab;     // Prefab de la bala

    [Header("Stats")]
    public float bulletSpeed = 10f;
    public float fireRate = 5f;         // Disparos por segundo

    private float nextFireTime = 0f;

    void Update()
    {
        // Dispara mientras mantienes pulsado el botón izquierdo
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Vector3 targetPos = GetMouseWorldPosition();
            Vector3 dir = (targetPos - firePoint.position).normalized;
            Shoot(dir);
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void Shoot(Vector3 direction)
    {
        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        var bullet = b.GetComponent<Bullet>();
        bullet.Initialize(new Vector2(direction.x, direction.z), bulletSpeed);
    }

    // Convierte la posición del cursor en pantalla a un punto en el plano Y = firePoint.position.y
    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * firePoint.position.y);
        if (groundPlane.Raycast(ray, out float enter))
            return ray.GetPoint(enter);
        return firePoint.position + transform.forward; // fallback
    }
}
