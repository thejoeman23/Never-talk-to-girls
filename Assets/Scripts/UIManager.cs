using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    private GameObject _interactButtonPrefab;
    private GameObject _currentInteractButton;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeInteractableObject(Interactable interactable)
    {
        // If there is nothing to interact with then clear the interact button
        if (interactable == null)
        {
            ClearInteractableButton();
            return;
        }
        
        GameObject obj = interactable.gameObject;
    }

    private void ClearInteractableButton()
    {
        
    }
}
