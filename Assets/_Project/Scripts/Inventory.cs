using System.Collections.Generic;
using _Project.Scripts;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemData> _items;

    public void Add(ItemData item) => _items.Add(item);
}