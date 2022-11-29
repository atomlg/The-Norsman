using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _minimumDistance;
    
    private Interactable _nearestInteractable;
    
    private void OnInteract()
    {
        if(_nearestInteractable == null)
            return;
        
        _nearestInteractable.Interact();
    }

    private void Update()
    {
        _nearestInteractable = null;
        Interactable[] interactables = FindObjectsOfType<Interactable>();
        for (int i = 0; i < interactables.Length; i++)
        {
            Interactable interactable = interactables[i];
            if (Distance(interactable.transform) > _minimumDistance)
                continue;

            if (_nearestInteractable == null || Distance(interactable.transform) < Distance(_nearestInteractable.transform))
                _nearestInteractable = interactable;
        }
    }

    public float Distance(Transform other) => Vector3.Distance(transform.position, other.position);
}
