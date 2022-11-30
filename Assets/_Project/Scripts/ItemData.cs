using UnityEngine;

namespace _Project.Scripts
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/Item Data", order = 0)]
    public class ItemData : ScriptableObject
    {
        [field:SerializeField] public string DisplayName { get; private set; }
    }
}