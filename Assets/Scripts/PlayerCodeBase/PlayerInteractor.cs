using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public Interactable currentInteractable;
    [SerializeField] InputActionReference interactAction;

    void OnTriggerEnter(Collider other)

    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
        }

    }
    void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable = null;
        }
    }
    void OnInteract(InputAction.CallbackContext context)
    {
       Debug.Log("Interact action performed");
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }
    private void OnEnable()
    {
        interactAction.action.Enable();
        interactAction.action.performed += OnInteract;
    }
    private void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
    }
}
