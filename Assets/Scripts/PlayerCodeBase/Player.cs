using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health = 100;
    public List<Item> inventory = new List<Item>();

    public void PickUpItem(Item item)
    {
        inventory.Add(item);
        item.PickUp();
    }

    void Update()
    {
        foreach (var item in inventory)
        {
            item.UpdateItem();
        }
    }
}