using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private void Start() => InstantiatePlayerCanvas();

    public void TransmitCharacter(Transform character) => InstantiateCharacterCanvas(character);
    
    public void BeginDialogue(DialogueGraph dialogue)
    {
        foreach (var node in dialogue.nodes)
        {
            if (node.IsStart)
            {
                Debug.Log("Dialogue Start");
                DisplayNode(node);
                return;
            }
        }
        
        Debug.LogWarning("Dialogue start not found");
    }
    
    private void DisplayNode(DialogueNode node)
    {
        DisplayText(node);

        node.Event?.Invoke();
        _audioSource.clip = node.Audio;
        
        if (node.IsEnd)
        {
            node.Event?.Invoke();
            EndDialogue();
            return;
        }
        
        if (node is ResponseNode response)
        {
            HideAllCanvases();
            DisplayNode(response.NextPrompt);
        }
        else if (node is PromptNode prompt)
        {
            DisplayOptions(prompt);
        }
    }

    private void EndDialogue()
    {
        Debug.Log("Dialogue end");
        
        // Hide UI
    }
    
    public void SelectOption(GameObject option)
    {
        HideAllCanvases();
        
        DialogueNode optionNode = _playerOptions[option];
        DisplayNode(optionNode);
    }
    
    /////////////////////// Entering Visuals Section ///////////////////////
    
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
    private Dictionary<GameObject, DialogueNode> _playerOptions;
    
    private GameObject _characterTextBubbleCanvas;
    private TextMeshProUGUI _characterTextBubble;

    private void InstantiatePlayerCanvas()
    {
        if (_playerTransform == null)
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        _playerTextBubbleCanvas = Instantiate(_textBubbleCanvasPrefab, _playerTransform);
        _playerTextBubbleCanvas.transform.localScale = Vector3.zero;
        _playerTextBubbleCanvas.GetComponent<RectTransform>().position = new Vector3(0, _playerTransform.localScale.y * 2, 0);
        
        _playerTextBubble = Instantiate(_textBubblePrefab, _playerTextBubbleCanvas.transform)
            .GetComponentInChildren<TextMeshProUGUI>();
        
        _playerOptionsCanvas = Instantiate(_optionsCanvasPrefab, _playerTransform);
        _playerOptionsCanvas.transform.localScale = Vector3.zero;
        _playerOptionsCanvas.transform.localPosition = new Vector3(0, _playerTransform.localScale.y * 2, 0);
    }
    
    private void InstantiateCharacterCanvas(Transform character)
    {
        _characterTransform = character;

        _characterTextBubbleCanvas = Instantiate(_textBubbleCanvasPrefab, _characterTransform);
        _characterTextBubbleCanvas.transform.localScale = Vector3.zero;
        _characterTextBubbleCanvas.transform.position = new Vector3(0, _characterTransform.localScale.y * 2, 0);

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
        if (node.Responses.Count == 0)
            EndDialogue();
        else if (node.Responses.Count == 1)
        {
            HideAllCanvases();
            DisplayNode(node.Responses[0]);
        }        
        else
        {
            _playerOptions.Clear();
            
            foreach (var response in node.Responses)
            {
                TextMeshProUGUI optionText = Instantiate(
                            _optionPrefab, 
                            _playerOptionsCanvas
                                .GetComponentInChildren<HorizontalLayoutGroup>()
                                .transform
                            ).GetComponent<TextMeshProUGUI>();
                optionText.text = response.Text;
                
                optionText.GetComponent<Button>().onClick.AddListener(() => SelectOption(optionText.gameObject));
                
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
