using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    private bool _confirmed = false;
    [SerializeField] private UnityEvent _onDialogueEnd;
    AudioSource _dialogueSource;

    private void Start()
    {
        InstantiatePlayerCanvas();
        _dialogueSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_confirmAction.action.triggered)
        {
            _confirmed = true;
        }
        
        _playerTextBubbleCanvas.transform.LookAt(_cameraTransform);
        
        if (_characterTextBubbleCanvas != null)
            _characterTextBubbleCanvas.transform.LookAt(_cameraTransform);
    }

    // Called by Interactables alongside a BeginDialogue() call. Spawns character canvas ahead of time.
    public void TransmitCharacter(Transform character) => InstantiateCharacterCanvas(character);
    
    public void BeginDialogue(DialogueGraph dialogue)
    {
        if (dialogue.start == null)
        {
            Debug.LogWarning("Dialogue tree does not contain a start node.");
            return;
        }

        if (dialogue.start.NextNodes.Count == 0)
        {
            Debug.LogWarning("Dialogue start node is not connected to anything.");
            return;
        }

        // If start node is connected to only one other node, assume player is not talking
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
        if (baseNode == null)
        {
            Debug.LogWarning("Node is null. Ending Dialogue.");
            EndDialogue();
            yield break;
        }
        
        if (baseNode is EndNode endNode)
        {
            EndDialogue(endNode);
            yield break;
        }
        
        DialogueNode node = baseNode as DialogueNode;
        DisplayText(node);
        
        AudioManager.Instance.MuteMusic();

        if (node.AudioClip == null)
        {
            yield return new WaitForSeconds(2);
        }
        else
        {
            _dialogueSource.clip = node.AudioClip;
            _dialogueSource.Play();
        
            while (_dialogueSource.isPlaying)
            {
                yield return new WaitForSeconds(.1f);
            }
            
            yield return new WaitForSeconds(.5f);
        }
        
        HideAllCanvases();
        
        if (node.NextNodes.Count == 0)
        {
            Debug.LogWarning("Dialogue node is not connected to anything. Ending Dialogue.");
            EndDialogue();
        }
        else if (node.NextNodes.Count == 1)
        {
            StartCoroutine(DisplayNode(node.NextNodes[0]));
        }
        else
        {
            DisplayOptions(node.NextNodes);
        }
    }

    private void EndDialogue(EndNode endNode = null)
    {
        HideAllCanvases();
        Destroy(_characterTextBubbleCanvas);
        
        GameManager.Instance.ChangeState(GameManager.GameState.GameOver);
        
        if (endNode != null)
            EndscreenManager.Instance.DisplayNewEndscreen(endNode);
        
        _onDialogueEnd.Invoke();
    }
    
    public void SelectOption(GameObject option)
    {
        HideAllCanvases();
        
        DialogueNode optionNode = _playerOptions[option];
        StartCoroutine(DisplayNode(optionNode));
    }
    
    /////////////////////// Entering Visuals Section ///////////////////////
    
    [Header("Input Actions")]
    [SerializeField] private InputActionReference _confirmAction;
    
    [Header("Visual Prefab References")]
    [SerializeField] private GameObject _optionPrefab;
    [SerializeField] private GameObject _textBubblePrefab;
    [SerializeField] private GameObject _textBubbleCanvasPrefab;
    
    [Header("Player Dialogue Options Canvas")]
    [SerializeField] private GameObject _optionsCanvas;

    [Header("UI Positioning")] 
    [SerializeField] private float _verticalCanvasOffset = 5;
    
    [Header("Tweening Settings")]
    [SerializeField] public float _tweenTime = 0.1f;
    [SerializeField] public Ease _tweenEase = Ease.Linear;
    
    private Transform _characterTransform;
    private Transform _playerTransform;

    private GameObject _playerTextBubbleCanvas;
    private TextMeshProUGUI _playerTextBubble;
    private Dictionary<GameObject, DialogueNode> _playerOptions = new Dictionary<GameObject, DialogueNode>();
    
    private GameObject _characterTextBubbleCanvas;
    private TextMeshProUGUI _characterTextBubble;
    
    private Transform _cameraTransform;

    private void InstantiatePlayerCanvas()
    {
        _cameraTransform = Camera.main?.transform;
        
        if (_playerTransform == null)
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        _playerTextBubbleCanvas = Instantiate(_textBubbleCanvasPrefab, _playerTransform);
        _playerTextBubbleCanvas.transform.localScale = Vector3.zero;
        _playerTextBubbleCanvas.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, GetMeshTopY(_playerTransform.gameObject) * 3, 0);
        _playerTextBubbleCanvas.transform.LookAt(Camera.main?.transform);
        
        _playerTextBubbleCanvas.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("RenderCamera").GetComponent<Camera>();
        
        _playerTextBubble = Instantiate(_textBubblePrefab, _playerTextBubbleCanvas.transform)
            .GetComponentInChildren<TextMeshProUGUI>();
    }
    
    private void InstantiateCharacterCanvas(Transform character)
    {
        _characterTransform = character;

        _characterTextBubbleCanvas = Instantiate(_textBubbleCanvasPrefab, _characterTransform);
        _characterTextBubbleCanvas.transform.localScale = Vector3.zero;
        _characterTextBubbleCanvas.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 1);
        _characterTextBubbleCanvas.transform.LookAt(Camera.main?.transform);
        
        _characterTextBubble = Instantiate(_textBubblePrefab, _characterTextBubbleCanvas.transform)
            .GetComponentInChildren<TextMeshProUGUI>();
    }

    private void DisplayText(DialogueNode node)
    {
        // If node contains multiple next node, that means the player will have options on how to respond
        // therefore we can deduce it is the character speaking
        if (node.NextNodes.Count > 1)
        {
            _characterTextBubbleCanvas.SetActive(true);
            _characterTextBubble.text = node.Text;
        }
        else
        {
            _playerTextBubbleCanvas.SetActive(true);
            _playerTextBubble.text = node.Text;
        }
    }

    private void DisplayOptions(List<BaseNode> options)
    {
        if (_optionsCanvas == null)
        {
            Debug.LogWarning("Options canvas is null.");
            return;
        }
        
        // Get object in which options will be spawned
        Transform optionsParent = _optionsCanvas
            .GetComponentInChildren<HorizontalLayoutGroup>()
            .transform;
        
        // Ensures previous options and option buttons are removed
        ClearPreviousOptions();
        
        // Enable options canvas
        _optionsCanvas.SetActive(true);
        
        foreach (var optionNode in options)
        {
            if (optionNode == null)
                continue;

            if (optionNode is EndNode)
            {
                EndDialogue(optionNode as EndNode);
                return;
            }

            DialogueNode option = optionNode as DialogueNode;

            GameObject optionButton = Instantiate(
                _optionPrefab,
                optionsParent
            );
            
            TextMeshProUGUI optionText = optionButton.GetComponentInChildren<TextMeshProUGUI>();
            Button optionButtonButton = optionButton.GetComponent<Button>();
            
            optionText.text = option.Text;
            optionButtonButton.onClick.AddListener(() => SelectOption(optionText.gameObject));
            
            _playerOptions.Add(optionText.gameObject, option);
        }
    }

    private void HideAllCanvases()
    {
        HideCanvas(_playerTextBubbleCanvas);
        HideCanvas(_characterTextBubbleCanvas);
        HideCanvas(_optionsCanvas);
    }

    private void HideCanvas(GameObject canvas)
    {
        if (canvas == null) return; // not created yet
    
        var worldCanvas = canvas.GetComponent<WorldCanvas>();
        if (worldCanvas != null)
            worldCanvas.Hide();
        else
            canvas.SetActive(false); // fallback if component missing
    }

    private void ClearPreviousOptions()
    {
        // Clear UI Options
        foreach (var pair in _playerOptions)
        {
            Destroy(pair.Key.transform.parent.gameObject);
        }
        
        // Clear option variables
        _playerOptions.Clear();
    }
    
    private float GetMeshTopY(GameObject go)
    {
        SkinnedMeshRenderer mf = go.GetComponentInChildren<SkinnedMeshRenderer>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogWarning("GameObject has no MeshFilter or mesh.");
            return go.transform.position.y; // fallback to pivot position
        }

        Mesh mesh = mf.sharedMesh;
        Bounds bounds = mesh.bounds;

        // Top of mesh in local space
        Vector3 localTop = new Vector3(0f, bounds.max.y, 0f);

        // Convert to world space
        Vector3 worldTop = go.transform.TransformPoint(localTop);

        return worldTop.y;
    }
}
