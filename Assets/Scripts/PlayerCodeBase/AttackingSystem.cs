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
            Vector2 inputDir = attackAction.action.ReadValue<Vector2>();
            if (inputDir.sqrMagnitude <= 0f)
            {
                // if stick returned to zero, keep last known direction
                inputDir = lastInputDir;
            }
            else
            {
                lastInputDir = inputDir;
            }
            Bullet bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity).GetComponent<Bullet>();
            bullet.SpawnBullet(inputDir, bulletSpeed, bulletLifetime);
            //  SpawnBullet(inputDir);
            yield return new WaitForSeconds(fireInterval);
        }
        attackCoroutine = null;
    }

    // Helper to spawn a bullet using same mapping and rb handling as before


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
