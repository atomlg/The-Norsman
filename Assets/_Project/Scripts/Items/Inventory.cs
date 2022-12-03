using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Items
{
    public class Inventory : MonoBehaviour
    {
        private List<ItemData> _items = new List<ItemData>();

        public void Add(ItemData item) => _items.Add(item);
    }
}