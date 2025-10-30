using UnityEngine;
using UnityEngine.InputSystem;

public class AttackingSystem : MonoBehaviour
{
    [SerializeField] InputActionReference attackAction;
    public GameObject BulletPrefab;

    // Added configurable speed & lifetime
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] float bulletLifetime = 5f;

    void OnAttack(InputAction.CallbackContext context)
    {
        // Read a Vector2 from the action context
        Vector2 inputDir = context.ReadValue<Vector2>();
        if (inputDir.sqrMagnitude <= 0f) return;

        // Instantiate bullet at this object's position
        GameObject bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);

        // If bullet has a Rigidbody2D, use it
        var rb2d = bullet.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.linearVelocity = inputDir.normalized * bulletSpeed;
            Destroy(bullet, bulletLifetime);
            return;
        }

        // If bullet has a 3D Rigidbody, use that
        var rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir3 = new Vector3(inputDir.x, inputDir.y, 0f).normalized;
            rb.linearVelocity = dir3 * bulletSpeed;
            Destroy(bullet, bulletLifetime);
            return;
        }

        // Fallback: move the transform slightly and still destroy after lifetime
        bullet.transform.position += (Vector3)inputDir.normalized * 0.01f;
        Destroy(bullet, bulletLifetime);
    }

    private void OnEnable()
    {
        if (attackAction?.action == null) return;
        attackAction.action.Enable();
        // Subscribe to performed to get the Vector2 value reliably
        attackAction.action.performed += OnAttack;
    }

    private void OnDisable()
    {
        if (attackAction?.action == null) return;
        attackAction.action.performed -= OnAttack;
    }
}
