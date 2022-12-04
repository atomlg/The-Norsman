using System.Collections;
using UnityEngine;

namespace _Project.Scripts.Interactions
{
    public class Interactor : MonoBehaviour
    {
        private const float UpdateInteractableAfterSeconds = 0.2f;
    
        [SerializeField] private float _minimumDistance;
    
        private Interactable _nearestInteractable;
        private Interactable[] _allInteractables;
        private WaitForSeconds _updateInteractableAfter;

        private void Awake()
        {
            _allInteractables = FindObjectsOfType<Interactable>();
            _updateInteractableAfter = new WaitForSeconds(UpdateInteractableAfterSeconds);
        }

        private void Start() => StartCoroutine(UpdateNearestInteractableCoroutine());

        private void OnInteract()
        {
            if(_nearestInteractable == null)
                return;
        
            _nearestInteractable.Interact();
        }
    
        private IEnumerator UpdateNearestInteractableCoroutine()
        {
            for (;;)
            {
                _nearestInteractable = null;
                for (int i = 0; i < _allInteractables.Length; i++)
                {
                    Interactable interactable = _allInteractables[i];
                    if (Distance(interactable.transform) > _minimumDistance)
                        continue;

                    if (_nearestInteractable == null || Distance(interactable.transform) < Distance(_nearestInteractable.transform))
                        _nearestInteractable = interactable;
                }

                yield return _updateInteractableAfter;
            }
        }
    
        private float Distance(Transform other) => Vector3.Distance(transform.position, other.position);
    }
}
