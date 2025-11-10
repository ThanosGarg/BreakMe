using UnityEngine;

public class Chest :    Interactable
{
    public override void Interact()
    {
        base.Interact();
        Debug.Log("Chest opened!");
        // Add chest opening logic here (e.g., play animation, give items, etc.)
    }
}
