using System.Collections.Generic;
using _Project.Scripts;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<ItemData> _items = new List<ItemData>();

    public void Add(ItemData item) => _items.Add(item);
}