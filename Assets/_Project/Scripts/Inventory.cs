using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<string> _items;

    public void Add(string item) => _items.Add(item);
}