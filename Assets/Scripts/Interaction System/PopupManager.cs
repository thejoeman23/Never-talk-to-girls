using UnityEngine;
using DG.Tweening;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [Header("Interact Popup Settings")]
    [SerializeField] float _interactButtonSize = 0.5f;
    [SerializeField] private float _heightOffset = 2;
    [SerializeField] private GameObject _interactButtonPrefab;
    private GameObject _currentInteractButton;
    
    [Header("Tween Settings")]
    [SerializeField] float _tweenDuration = 0.5f;
    private Sequence _currentTween;

    private void Awake()
    {
        Instance = this;
        _currentTween = DOTween.Sequence();
    }

    public void ChangeInteractableObject(Interactable interactable)
    {
        if (_currentInteractButton == null)
        {
            CreateFirstInteractButton(interactable);
        }
        
        // If there is nothing to interact with then clear the interact button
        if (interactable == null)
        {
            ClearInteractableButton();
            return;
        }

        GameObject oldButton = _currentInteractButton;
        _currentInteractButton = CreateInteractButton(interactable);
        
        // Kill the old tween and create a new one
        _currentTween.Kill();
        _currentTween = DOTween.Sequence();

        // Tween away the old button then tween in the new one
        _currentTween.Append(oldButton.transform.DOScale(Vector3.zero, _tweenDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => Destroy(oldButton)));
        _currentTween.Append(_currentInteractButton.transform.DOScale(Vector3.one * _interactButtonSize, _tweenDuration)
            .SetEase(Ease.Linear));

        _currentTween.Play();
    }

    private GameObject CreateInteractButton(Interactable interactable)
    {
        GameObject button = Instantiate(_interactButtonPrefab, transform.parent);
        button.transform.position = interactable.transform.position + new Vector3(0, (interactable.transform.localScale.y / 2) + _heightOffset, 0);
        button.transform.localScale = Vector3.zero;
        
        return button;
    }

    private void ClearInteractableButton()
    {
        _currentTween.Kill();
        _currentTween = DOTween.Sequence();

        _currentTween.Append(_currentInteractButton.transform.DOScale(Vector3.zero, _tweenDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => Destroy(_currentInteractButton)));
        
        _currentTween.Play();
    }

    private void CreateFirstInteractButton(Interactable interactable)
    {
        _currentInteractButton = CreateInteractButton(interactable);
        
        _currentTween.Kill();
        _currentTween = DOTween.Sequence();
        
        _currentTween.Append(_currentInteractButton.transform.DOScale(Vector3.one * _interactButtonSize, _tweenDuration)
            .SetEase(Ease.Linear));
    }
}
