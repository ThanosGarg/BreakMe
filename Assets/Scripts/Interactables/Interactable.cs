using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
   public virtual void Interact()
   {
       Debug.Log("Interacting with " + gameObject.name);
   }
}
