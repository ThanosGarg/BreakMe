using UnityEngine;

[CreateAssetMenu(fileName = "NewTestItem", menuName = "Items/TestItem")]
public class TestItem : Item
{
    public override void PickUp()
    {
        base.PickUp();
        Debug.Log($"TestItem specific pick up logic for: {itemName}");
    }

    public override void UpdateItem()
    {
        base.UpdateItem();
        Debug.Log($"TestItem specific update logic for: {itemName}");
    }
}
