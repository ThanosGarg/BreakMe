using UnityEngine;

public class ItemHolder : Interactable
{
    public Item item;
    public override void Interact()
    {
        base.Interact();
       GameManger.instance.player.PickUpItem(item);
    }
}
