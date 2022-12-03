using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private List<InteractionHandler> _handlers;

    private void Awake() => _handlers = GetComponentsInChildren<InteractionHandler>().ToList();

    public void Interact()
    {
        foreach (InteractionHandler handler in _handlers) 
            handler.Interact();
    }
}