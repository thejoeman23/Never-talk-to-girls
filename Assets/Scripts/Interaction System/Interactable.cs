using System.Collections;

namespace InteractionSystem
{
    using UnityEngine;
    using UnityEngine.Events;

    [AddComponentMenu("Interaction System/Interactable")]
    [RequireComponent(typeof(Collider))]
    public class Interactable : MonoBehaviour, IInteractable
    {
        [Header("Interactable Settings")]
        [Tooltip("Whether this object can currently be interacted with.")]
        [SerializeField] private bool canInteract = true;
        [Tooltip("The time the player has to wait before interacting with this object again.")]
        [SerializeField] private float interactCooldown = 5;

        [Tooltip("Optional height offset for UI prompts (gizmos only).")]
        [SerializeField] private float interactPromptHeight = 1f;

        [Header("Events")]
        [Tooltip("Events fired when the player interacts with this object.")]
        public UnityEvent onInteract = new UnityEvent();
        
        [Header("Gizmos Settings")]
        [Tooltip("Draw the Gizmos for the interactable objects.")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color gizmosColor = Color.yellow;

        public bool CanInteract => canInteract;
        public float InteractPromptHeight => interactPromptHeight;

        private void Reset()
        {
            gameObject.tag = "Interactable";
        }

        public void Interact()
        {
            if (!canInteract) return;

            onInteract.Invoke();
            InteractCooldown();
        }

        private void InteractCooldown() => StartCoroutine(Cooldown());
        
        private IEnumerator Cooldown()
        {
            canInteract = false;
            yield return new WaitForSeconds(interactCooldown);
            canInteract = true;
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos)
                return;

            Gizmos.color = gizmosColor;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * interactPromptHeight);
        }
    }
    
    public interface IInteractable
    {
        void Interact();
    }
}
