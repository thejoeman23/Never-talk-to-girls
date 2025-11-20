using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Events;

[RequireComponent(typeof(DialogueVisualManager))]
public class DialogueManager : MonoBehaviour
{
    private DialogueVisualManager _visuals;
    
    [Header("References")]
    [SerializeField] private InputActionReference _confirmAction;
    
    [Header("Events")]
    [SerializeField] private UnityEvent _onDialogueEnd;

    [Header("Audio Variables")]
    [Tooltip("The of time a text buble is displayed before continuing if no dialogue audio is found.")]
    [SerializeField] private float _waitTimeIfAudioIsNull = 2f;
    [Tooltip("A time padding between when the dialogue audio is played and when the conversation continues.")]
    [SerializeField] private float _paddingTime = .5f;
    
    private bool _confirmed = false; // Will be used as a skip button later on
    AudioSource _dialogueSource;

    private void Start()
    {
        _visuals = GetComponent<DialogueVisualManager>();
        
        // Catch if visual reference is not given. Visuals are necessary to this system.
        if (_visuals == null)
        {
            Debug.LogError("Dialogue Manager is has no reference to DialogueVisualManager. Disabling Script.");
            this.enabled = false;
        }
        
        _dialogueSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_confirmAction.action.triggered)
        {
            _confirmed = true;
        }
    }

    // Called by Interactables alongside a BeginDialogue() call. Spawns character canvas ahead of time.
    public void CreateNpcCanvas(Transform character) => _visuals.InstantiateCharacterCanvas(character);

    public void BeginDialogue(DialogueGraph dialogue)
    {
        // Catch if node is null
        if (dialogue.start == null)
        {
            Debug.LogWarning("Dialogue tree does not contain a start node.");
            return;
        }

        // Catch if nothing comes after the start
        if (dialogue.start.NextNodes.Count == 0)
        {
            Debug.LogWarning("Dialogue start node is not connected to anything.");
            return;
        }

        // If start node is connected to only one other node, assume NPC is talking and display their dialogue
        if (dialogue.start.NextNodes.Count == 1)
        {
            StartCoroutine(DisplayNode(dialogue.start.NextNodes[0]));
        }
        else // Else give player options on how to begin the conversation
        {
            DisplayOptions(dialogue.start.NextNodes);
        }
        
        GameManager.Instance.ChangeState(GameManager.GameState.Rizzing);
    }

    private IEnumerator DisplayNode(BaseNode baseNode)
    {
        // Catch if the node is null somehow
        if (baseNode == null)
        {
            Debug.LogWarning("Node is null. Ending Dialogue.");
            EndDialogue();
            yield break;
        }

        // Catch if its an end node, in which case end the dialogue
        if (baseNode is EndNode endNode)
        {
            EndDialogue(endNode);
            yield break;
        }

        DialogueNode node = baseNode as DialogueNode;
        
        // Hide all UI and Mute Background Music
        HideUI();
        AudioManager.Instance.MuteMusic();
        
        // Show text bubble popup
        VisualizeNode(node);
        
        // Play voice recording
        #region Playing Audio

        if (node.AudioClip == null)
        {
            yield return new WaitForSeconds(_waitTimeIfAudioIsNull);
        }
        else
        {
            _dialogueSource.clip = node.AudioClip;
            _dialogueSource.Play();

            yield return new WaitUntil(() => _dialogueSource.isPlaying);

            yield return new WaitForSeconds(_paddingTime);
        }

        #endregion
        
        switch(node.NextNodes.Count)
        {
            // Catch in case there are no next nodes
            case 0:
                Debug.LogWarning("Dialogue node is not connected to anything. Ending Dialogue.");
                EndDialogue();
                break;
            
            // If there is just one next node then it is the NPC responding, so display their response
            case 1:
                StartCoroutine(DisplayNode(node.NextNodes[0]));
                break;
            
            // If there are more, allow the player to choose what they say next
            default:
                DisplayOptions(node.NextNodes);
                break;
        }
    }

    private void EndDialogue(EndNode endNode = null)
    {
        // Clear all UI
        HideUI();
        _visuals.DestroyNpcCanvas();
        
        // Display endscreen
        EndscreenManager.Instance.DisplayNewEndscreen(endNode);
        
        _onDialogueEnd.Invoke();
    }
    
    private void VisualizeNode(DialogueNode node) => _visuals.DisplayTextBubble(node);
    
    private void DisplayOptions(List<BaseNode> options) => _visuals.DisplayOptions(options, SelectOption);

    private void SelectOption(BaseNode option) => StartCoroutine(DisplayNode(option));

    private void HideUI() => _visuals.HideAllCanvases();
}