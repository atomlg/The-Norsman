using _Project.Scripts;
using UnityEngine;

public class CollectItem: InteractionHandler
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private ItemData _item;
    
    public override void Interact()
    {
        _inventory.Add(_item);
        Debug.Log("Item collected");
    }
}