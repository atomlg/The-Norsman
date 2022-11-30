using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private List<InteractionHandler> _handlers;
    
    public void Interact()
    {
        foreach (InteractionHandler handler in _handlers) 
            handler.Interact();
    }
}