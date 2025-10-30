using UnityEngine;

public class Door : Interactable
{
  public override void Interact()
  {
    base.Interact();
    Debug.Log("The door " + gameObject.name + " has been opened!");
    // Additional door opening logic can be added here
  }
}
