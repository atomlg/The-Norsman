using _Project.Scripts.Items;
using UnityEngine;

namespace _Project.Scripts.Interactions
{
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
}