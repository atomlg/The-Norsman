using UnityEngine;

public class Inventory : MonoBehaviour
{
    [field: SerializeField] public string[] Items { get; private set; }
}