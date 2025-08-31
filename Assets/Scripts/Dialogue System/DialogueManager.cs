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

    public void TransmitCharacter(Transform character) => InstantiateCharacterCanvas(character);
    
    public void BeginDialogue(DialogueGraph dialogue)
    {
        List<ResponseNode> starts = new List<ResponseNode>();
        foreach (var node in dialogue.nodes)
        {
            if (node.IsStart && node is ResponseNode response)
            {
                Debug.Log("Dialogue Start");
                starts.Add(response);
            }
            else if (node.IsStart)
            {
                StartCoroutine(DisplayNode(node));
                return;
            }
        }
        
        DisplayOptions(starts);
    }
    
    private IEnumerator DisplayNode(DialogueNode node)
    {
        HideAllCanvases();
        _confirmed = false;

        DisplayText(node);

        node.Event?.Invoke();
        
        AudioManager.Instance.MuteMusic();

        if (node.Audio == null)
        {
            yield return new WaitForSeconds(2);
        }
        else
        {
            _dialogueSource.clip = node.Audio;
            _dialogueSource.Play();
        
            while (_dialogueSource.isPlaying)
            {
                yield return new WaitForSeconds(.1f);
            }
            
            yield return new WaitForSeconds(.5f);
        }
        
        if (node.IsEnd)
        {
            if (node.EndSprite != null)
            {
                EndscreenManager.Instance.DisplayNewEndscreen(node.EndSprite);
            }

            EndDialogue();
            yield return null;
        }
        
        if (node is ResponseNode response)
        {
            StartCoroutine(DisplayNode(response.NextPrompt));
        }
        else if (node is PromptNode prompt)
        {
            DisplayOptions(prompt.Responses);
        }
    }

    private void EndDialogue()
    {
        Debug.Log("Dialogue end");
        
        HideAllCanvases();
        Destroy(_characterTextBubbleCanvas);
        
        _onDialogueEnd.Invoke();
    }
    
    public void SelectOption(GameObject option)
    {
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
        if (node is ResponseNode response)
        {
            _playerTextBubbleCanvas.SetActive(true);
            _playerTextBubble.text = response.Text;
        }
        else
        {
            _characterTextBubbleCanvas.SetActive(true);
            _characterTextBubble.text = node.Text;
        }
    }

    private void DisplayOptions(List<ResponseNode> responses)
    {
        if (responses == null)
            EndDialogue();
        
        if (responses.Count == 0)
            EndDialogue();
        else if (responses.Count == 1)
        {
            StartCoroutine(DisplayNode(responses[0]));
        }        
        else
        {
            _playerOptions.Clear();
            
            _optionsCanvas.SetActive(true);
            Transform optionsParent = _optionsCanvas
                .GetComponentInChildren<HorizontalLayoutGroup>()
                .transform;

            foreach (Transform child in optionsParent.transform)
            {
                Destroy(child.gameObject);
            }
            
            foreach (var response in responses)
            {
                if (response == null)
                    continue;
                
                GameObject optionButton = Instantiate(
                    _optionPrefab,
                    optionsParent
                );
                TextMeshProUGUI optionText = optionButton.GetComponentInChildren<TextMeshProUGUI>();
                Button optionButtonButton = optionButton.GetComponent<Button>();
                
                optionText.text = response.Text;
                optionButtonButton.onClick.AddListener(() => SelectOption(optionText.gameObject));
                
                _playerOptions.Add(optionText.gameObject, response);
            }
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
    
    public float GetMeshTopY(GameObject go)
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
