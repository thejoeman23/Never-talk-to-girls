using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))] // If there is no collider it cant be seen by the Interactor
public class Interactable : MonoBehaviour, IInteractable // <- See how it derives?
{
    // An event called on interacting with this object
    [SerializeField] public UnityEvent _onInteract = new UnityEvent();
    [SerializeField] public bool _canInteract = true;

    public float interactButtonHeight;

    public void Start()
    {
        gameObject.tag = "Interactable";
    }
    
    // Call the function
    public void Interact()
    {
        // If _canInteract is false then cut the function short and dont run the rest of the code
        if (!_canInteract)
            return;
        
        _onInteract.Invoke();
        
        _canInteract = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, interactButtonHeight, 0));
    }
}

// An interface is a list of functions that whatever derives from it must contain
// Deriving is a way of inheriting variables and functions from another script, or in this case, interface
public interface IInteractable
{
    public void Interact();
}
