using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] InputActionReference interactKey;
    [SerializeField] private float _interactRadius = 5;
    
    Interactable _nearestInteractableObject;

    private void Start()
    {
        _nearestInteractableObject = null;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNearestInteractableObject();
        
        // If you press the interact button then interact
        if (interactKey.action.triggered && _nearestInteractableObject != null)
        { 
            _nearestInteractableObject.Interact();
        }
    }
    
    void UpdateNearestInteractableObject()
    {
        // Finds all colliders in a specified radius around the player.
        Vector3 spherePosition = transform.position - new Vector3(0, transform.localScale.y / 2, 0);
        Collider[] colliders = Physics.OverlapSphere(spherePosition, _interactRadius);

        // Loop through the list of all objects in the radius and find the nearest one that is Interactable
        float nearestDistance = Mathf.Infinity;
        Interactable nearestInteractableObject = null;
        foreach (Collider collider in colliders)
        {
            // Checks if the object contains an Interactable and if so assign it to the "interactable" variable
            if (collider.gameObject.TryGetComponent<Interactable>(out Interactable interactable) && interactable._canInteract)
            {
                float distance = Vector3.Distance(collider.transform.position, transform.position);

                // Check if the distance between it and the player is closer than the previous closest object
                if (distance < nearestDistance)
                {
                    // Set this object as the nearest Interactable
                    nearestDistance = distance;
                    nearestInteractableObject = interactable;
                }
            }
        }
        
        // Update the variable if there is a difference between the two
        if (nearestInteractableObject != _nearestInteractableObject)
        {
            _nearestInteractableObject = nearestInteractableObject;
            PopupManager.Instance.ChangeInteractableObject(_nearestInteractableObject);
        }
    }

    private void OnDrawGizmos() // Draw visuals in the scene view to show the interact radius
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _interactRadius);
    }
}
