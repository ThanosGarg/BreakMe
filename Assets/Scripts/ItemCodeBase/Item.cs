using UnityEngine;

public abstract class Item : ScriptableObject
{
    //Strategy pattern could be implemented here for different item behaviors
    public string itemName;

    public Sprite itemIcon;

    public virtual void PickUp()
    {
        Debug.Log($"Picked up {itemName}");
    }

    public virtual void UpdateItem()
    {
        Debug.Log($"Updating item: {itemName}");
    }
}