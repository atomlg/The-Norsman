using UnityEngine;

public class CollectItem: InteractionHandler
{
    public Inventory Inventory;
    public string Item;
    
    public override void Interact()
    {
        Inventory.Add(Item);
        Debug.Log("Item collected");
    }
}