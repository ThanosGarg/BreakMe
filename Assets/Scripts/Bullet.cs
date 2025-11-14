using UnityEngine;

public class Bullet : MonoBehaviour
{
    public void SpawnBullet(Vector2 inputDir, float bulletSpeed, float bulletLifetime)
    {

        if (inputDir.sqrMagnitude <= 0f) return;

        Vector3 dir3 = (transform.right * inputDir.x + transform.forward * inputDir.y);
        Vector3 dir3Norm = dir3.normalized;
        Vector3 spawnPos = transform.position + dir3Norm * 0.5f;
        //GameObject bullet = Instantiate(BulletPrefab, spawnPos, Quaternion.identity);

        var rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            Vector2 dir2 = (Vector2)(transform.right * inputDir.x + transform.up * inputDir.y);
            rb2d.linearVelocity = dir2.normalized * bulletSpeed;
            Destroy(gameObject, bulletLifetime);
            return;
        }

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = dir3Norm * bulletSpeed;
            Destroy(gameObject, bulletLifetime);
            return;
        }

        transform.position += dir3Norm * 0.01f;
        Destroy(gameObject, bulletLifetime);
    }
}
