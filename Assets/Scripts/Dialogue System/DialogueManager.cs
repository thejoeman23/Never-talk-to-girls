using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    private bool _confirmed = false;
    
    private void Start() => InstantiatePlayerCanvas();

    private void Update()
    {
        if (_confirmAction.action.triggered)
        {
            _confirmed = true;
        }
    }

    public void TransmitCharacter(Transform character) => InstantiateCharacterCanvas(character);
    
    public void BeginDialogue(DialogueGraph dialogue)
    {
        foreach (var node in dialogue.nodes)
        {
            if (node.IsStart)
            {
                Debug.Log("Dialogue Start");
                StartCoroutine(DisplayNode(node));
                return;
            }
        }
        
        Debug.LogWarning("Dialogue start not found");
    }
    
    private IEnumerator DisplayNode(DialogueNode node)
    {
        HideAllCanvases();
        _confirmed = false;

        DisplayText(node);

        node.Event?.Invoke();
        _audioSource.clip = node.Audio;
        _audioSource.Play();

        while (_audioSource.isPlaying == false && !_confirmed)
        {
            yield return new WaitForSeconds(.1f);
        }
        
        if (node.IsEnd)
        {
            EndDialogue();
            yield return null;
        }
        
        if (node is ResponseNode response)
        {
            StartCoroutine(DisplayNode(response.NextPrompt));
        }
        else if (node is PromptNode prompt)
        {
            DisplayOptions(prompt);
        }
    }

    private void EndDialogue()
    {
        Debug.Log("Dialogue end");
        
        HideAllCanvases();
        Destroy(_characterTextBubbleCanvas);
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
    [SerializeField] private GameObject _optionsCanvasPrefab;
    [SerializeField] private GameObject _textBubblePrefab;
    [SerializeField] private GameObject _textBubbleCanvasPrefab;
    
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;

    [Header("UI Positioning")] 
    [SerializeField] private float _verticalCanvasOffset = 5;
    
    [Header("Tweening Settings")]
    [SerializeField] public float _tweenTime = 0.1f;
    [SerializeField] public Ease _tweenEase = Ease.Linear;
    
    private Transform _characterTransform;
    private Transform _playerTransform;

    private GameObject _playerTextBubbleCanvas;
    private TextMeshProUGUI _playerTextBubble;
    private GameObject _playerOptionsCanvas;
    private Dictionary<GameObject, DialogueNode> _playerOptions = new Dictionary<GameObject, DialogueNode>();
    
    private GameObject _characterTextBubbleCanvas;
    private TextMeshProUGUI _characterTextBubble;

    private void InstantiatePlayerCanvas()
    {
        if (_playerTransform == null)
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        _playerTextBubbleCanvas = Instantiate(_textBubbleCanvasPrefab, _playerTransform);
        _playerTextBubbleCanvas.transform.localScale = Vector3.zero;
        _playerTextBubbleCanvas.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, _playerTransform.localScale.y * 2, 0);
        _playerTextBubbleCanvas.transform.LookAt(Camera.main?.transform);
        
        _playerTextBubble = Instantiate(_textBubblePrefab, _playerTextBubbleCanvas.transform)
            .GetComponentInChildren<TextMeshProUGUI>();
        
        _playerOptionsCanvas = Instantiate(_optionsCanvasPrefab, _playerTransform);
        _playerOptionsCanvas.transform.localScale = Vector3.zero;
        _playerOptionsCanvas.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, _playerTransform.localScale.y * 2, 0);
        _playerOptionsCanvas.transform.LookAt(Camera.main?.transform);
    }
    
    private void InstantiateCharacterCanvas(Transform character)
    {
        _characterTransform = character;

        _characterTextBubbleCanvas = Instantiate(_textBubbleCanvasPrefab, _characterTransform);
        _characterTextBubbleCanvas.transform.localScale = Vector3.zero;
        _characterTextBubbleCanvas.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, _characterTransform.localScale.y * 2, 0);
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

    private void DisplayOptions(PromptNode node)
    {
        if (node.Responses == null)
            EndDialogue();
        
        if (node.Responses.Count == 0)
            EndDialogue();
        else if (node.Responses.Count == 1)
        {
            StartCoroutine(DisplayNode(node.Responses[0]));
        }        
        else
        {
            _playerOptions.Clear();
            
            _playerOptionsCanvas.SetActive(true);
            Transform optionsParent = _playerOptionsCanvas
                .GetComponentInChildren<HorizontalLayoutGroup>()
                .transform;

            foreach (Transform child in optionsParent.transform)
            {
                Destroy(child.gameObject);
            }
            
            foreach (var response in node.Responses)
            {
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
        HideCanvas(_playerOptionsCanvas);
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
}
