using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class AttackingSystem : MonoBehaviour
{
    [SerializeField] InputActionReference attackAction;
    public GameObject BulletPrefab;

    // Added configurable speed & lifetime
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] float bulletLifetime = 5f;

    // New: fire interval (seconds) and state for hold-to-fire
    [SerializeField] float fireInterval = 0.2f;
    bool isAttacking = false;
    Coroutine attackCoroutine = null;
    Vector2 lastInputDir = Vector2.zero;

    // Called when the action is first started (button pressed / stick moved)
    void OnAttackStarted(InputAction.CallbackContext context)
    {
        isAttacking = true;
        lastInputDir = -context.ReadValue<Vector2>();
        if (attackCoroutine == null) attackCoroutine = StartCoroutine(AttackLoop());
    }

    // Called when the action is canceled (button released / stick released)
    void OnAttackCanceled(InputAction.CallbackContext context)
    {
        isAttacking = false;
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    IEnumerator AttackLoop()
    {
        while (isAttacking)
        {
            // Read current value each tick so player can change direction while holding
            Vector2 inputDir = -attackAction.action.ReadValue<Vector2>();
            if (inputDir.sqrMagnitude <= 0f)
            {
                // if stick returned to zero, keep last known direction
                inputDir = lastInputDir;
            }
            else
            {
                lastInputDir = inputDir;
            }

            SpawnBullet(inputDir);
            yield return new WaitForSeconds(fireInterval);
        }
        attackCoroutine = null;
    }

    // Helper to spawn a bullet using same mapping and rb handling as before
    void SpawnBullet(Vector2 inputDir)
    {
        if (inputDir.sqrMagnitude <= 0f) return;

        Vector3 dir3 = (transform.right * inputDir.x + transform.forward * inputDir.y);
        Vector3 dir3Norm = dir3.normalized;
        Vector3 spawnPos = transform.position + dir3Norm * 0.5f;
        GameObject bullet = Instantiate(BulletPrefab, spawnPos, Quaternion.identity);

        var rb2d = bullet.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            Vector2 dir2 = (Vector2)(transform.right * inputDir.x + transform.up * inputDir.y);
            rb2d.linearVelocity = dir2.normalized * bulletSpeed;
            Destroy(bullet, bulletLifetime);
            return;
        }

        var rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = dir3Norm * bulletSpeed;
            Destroy(bullet, bulletLifetime);
            return;
        }

        bullet.transform.position += dir3Norm * 0.01f;
        Destroy(bullet, bulletLifetime);
    }

    private void OnEnable()
    {
        if (attackAction?.action == null) return;
        attackAction.action.Enable();
        // Subscribe to started/canceled so we can hold the action
        attackAction.action.started += OnAttackStarted;
        attackAction.action.canceled += OnAttackCanceled;
    }

    private void OnDisable()
    {
        if (attackAction?.action == null) return;
        attackAction.action.started -= OnAttackStarted;
        attackAction.action.canceled -= OnAttackCanceled;
    }
}
